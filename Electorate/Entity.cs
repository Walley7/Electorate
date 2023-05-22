using Electorate.Blockchain;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class Entity {
        //================================================================================
        private ElectorateInstance              mElectorate;

        private string                          mDBTableName;
        private string                          mDBPrimaryKeyName;
        private object                          mDBPrimaryKeyValue;
        private string                          mDBPrimaryKeyName2;
        private object                          mDBPrimaryKeyValue2;

        private SqlDataReader                   mDBReader = null;


        //================================================================================
        //--------------------------------------------------------------------------------
        public Entity(ElectorateInstance electorate, string dbTableName, string dbPrimaryKeyName, object dbPrimaryKeyValue,
                      string dbPrimaryKeyName2 = null, object dbPrimaryKeyValue2 = null)
        {
            mElectorate = electorate;
            mDBTableName = dbTableName;
            mDBPrimaryKeyName = dbPrimaryKeyName;
            mDBPrimaryKeyValue = dbPrimaryKeyValue;
            mDBPrimaryKeyName2 = dbPrimaryKeyName2;
            mDBPrimaryKeyValue2 = dbPrimaryKeyValue2;
        }

        
        // ELECTORATE ================================================================================
        //--------------------------------------------------------------------------------
        public ElectorateInstance Electorate { get { return mElectorate; } }

        
        // DATABASE - SCHEMA ================================================================================
        //--------------------------------------------------------------------------------
        public string DBTableName { get { return mDBTableName; } }
        public string DBPrimaryKeyName { get { return mDBPrimaryKeyName; } }
        public object DBPrimaryKeyValue { get { return mDBPrimaryKeyValue; } }
        public string DBPrimaryKeyName2 { get { return mDBPrimaryKeyName2; } }
        public object DBPrimaryKeyValue2 { get { return mDBPrimaryKeyValue2; } }
        

        // DATABASE - READING ================================================================================
        //--------------------------------------------------------------------------------
        protected SqlDataReader DBOpenReader(string fields) {
            // Close
            DBCloseReader();

            // Open
            SqlCommand command = new SqlCommand("select " + fields + " from " + DBTableName + " where " + DBPrimaryKeyName + " = @" + DBPrimaryKeyName +
                                                (DBPrimaryKeyName2 != null ? " and " + DBPrimaryKeyName2 + " = @" + DBPrimaryKeyName2 : ""), Electorate.DatabaseConnection);
            command.Parameters.AddWithValue("@" + DBPrimaryKeyName, DBPrimaryKeyValue);
            if (DBPrimaryKeyName2 != null)
                command.Parameters.AddWithValue("@" + DBPrimaryKeyName2, DBPrimaryKeyValue2);
            mDBReader = command.ExecuteReader();

            // Read
            mDBReader.Read();

            // Return
            return mDBReader;
        }
        
        //--------------------------------------------------------------------------------
        protected void DBCloseReader() {
            if (mDBReader != null) {
                mDBReader.Close();
                mDBReader = null;
            }
        }

        //--------------------------------------------------------------------------------
        protected SqlDataReader DBReader { get { return mDBReader; } }
        protected bool DBHasReader { get { return (mDBReader != null); } }
        
        //--------------------------------------------------------------------------------
        protected DBField<T> DBRead<T>(ref DBField<T> field) {
            // Reader
            bool openedReader = false;
            if (!DBHasReader) {
                DBOpenReader(field.NamesString);
                openedReader = true;
            }

            // Read
            try {
                if (DBReader.HasRows)
                    field.Load(DBReader);
            }
            //catch (IndexOutOfRangeException) { return false; }
            finally {
                // Close reader
                if (openedReader)
                    DBCloseReader();
            }

            // Return
            return field;
        }
        
        //--------------------------------------------------------------------------------
        protected DBField<T> DBLoad<T>(ref DBField<T> field) {
            if (!field.Loaded)
                return DBRead(ref field);
            else
                return field;
        }

        //--------------------------------------------------------------------------------
        private object DBReplaceNull(object value, object replacement) {
            return (value != DBNull.Value ? value : replacement);
        }


        // DATABASE - WRITING ================================================================================
        //--------------------------------------------------------------------------------
        protected int SaveChanges(params IDBField[] fields) {
            // Query - set / where
            string querySet = "";
            string queryWhere = "";

            int i;
            foreach (IDBField f in fields) {
                if (f.Changed) {
                    i = 0;
                    foreach (string n in f.Names) {
                        // Set
                        if (!string.IsNullOrEmpty(querySet))
                            querySet += ",";
                        querySet += n + "=@" + n;

                        // Where
                        if (!string.IsNullOrEmpty(queryWhere))
                            queryWhere += " and ";
                        queryWhere += n + (f.OriginalValueObject(i) != DBNull.Value ? "=@Old" + n: " is null");
                        ++i;
                    }
                }
            }

            // Query
            if (string.IsNullOrEmpty(querySet) || string.IsNullOrEmpty(queryWhere))
                return 0;
            string query = "update " + DBTableName + " set " + querySet +
                           " where " + DBPrimaryKeyName + " = @" + DBPrimaryKeyName + (DBPrimaryKeyName2 != null ? " and " + DBPrimaryKeyName2 + " = @" + DBPrimaryKeyName2 : "") +
                           " and " + queryWhere;

            // Command
            SqlCommand command = new SqlCommand(query, Electorate.DatabaseConnection);

            // Parameters
            command.Parameters.AddWithValue("@" + DBPrimaryKeyName, DBPrimaryKeyValue);
            if (DBPrimaryKeyName2 != null)
                command.Parameters.AddWithValue("@" + DBPrimaryKeyName2, DBPrimaryKeyValue2);

            int count = 0;
            foreach (IDBField f in fields) {
                if (f.Changed) {
                    ++count;
                    i = 0;
                    foreach (string n in f.Names) {
                        command.Parameters.AddWithValue("@" + n, f.ValueObject(i));
                        if (f.OriginalValueObject(i) != DBNull.Value)
                            command.Parameters.AddWithValue("@Old" + n, f.OriginalValueObject(i));
                        ++i;
                    }
                }
            }

            // Save
            command.ExecuteNonQuery();
            command.Dispose();
            return count;
        }
        
        //--------------------------------------------------------------------------------
        /// <returns>The number of fields updated.</returns>
        public virtual int SaveChanges() { return 0; }


        //================================================================================
        //********************************************************************************
        protected interface IDBField {
            string[] Names { get; }
            string Name { get; }
            string NamesString { get; }
            object OriginalValueObject(int index);
            object ValueObject(int index);
            bool Loaded { get; }
            bool Changed { get; }
            bool IsNull { get; }
        }

        //********************************************************************************
        protected class DBField<T> : IDBField {
            private string[] mNames;
            protected object mOriginalValue = null;
            protected object mValue = null;

            public DBField(params string[] names) {
                if (default(T) != null)
                    throw new NotSupportedException("DBField type must be nullable.");
                mNames = names;
            }

            public string[] Names { get { return mNames; } }
            public string Name { get { return mNames[0]; } }
            public string NamesString { get { return string.Join(",", mNames); } }

            public void Load(object value) {
                mOriginalValue = value;
                mValue = value;
            }

            public virtual void Load(SqlDataReader reader) { Load(reader[Name]); }

            protected void SetValue(ref object memberValue, T value) { memberValue = (value != null ? (object)value : DBNull.Value); }
            
            // Conversion from primitives to nullable enums causes issues, hence the logic specific to them
            protected T GetValue(ref object memberValue) {
                if (memberValue != DBNull.Value) {
                    Type nullabledType = Nullable.GetUnderlyingType(typeof(T));
                    if ((nullabledType != null) && nullabledType.IsEnum)
                        return (T)Enum.ToObject(nullabledType, memberValue);
                    else
                        return (T)memberValue;
                }
                else
                    return default(T);
            }

            protected virtual object GetValueObject(ref object memberValue, int index) { return (memberValue != DBNull.Value ? GetValue(ref memberValue) : (object)DBNull.Value); }
            
            public T OriginalValue {
                set { SetValue(ref mOriginalValue, value); }
                get { return GetValue(ref mOriginalValue); }
            }

            public virtual object OriginalValueObject(int index = 0) { return GetValueObject(ref mOriginalValue, index); }

            public T Value {
                set { SetValue(ref mValue, value); }
                get { return GetValue(ref mValue); }
            }

            public virtual object ValueObject(int index = 0) { return GetValueObject(ref mValue, index); }

            public bool Loaded { get { return mOriginalValue != null; } }
            public bool Changed { get { return (Loaded && !mOriginalValue.Equals(mValue)); } }
            public virtual bool IsNull { get { return mValue == DBNull.Value; } }
        }

        //********************************************************************************
        protected class DBBlockchainKeyField : DBField<BlockchainKey> {
            public DBBlockchainKeyField(params string[] names) : base(names) { }

            public override void Load(SqlDataReader reader) {
                object publicKey = reader[Names[0]];
                publicKey = (publicKey != DBNull.Value ? publicKey : null);
                object privateKey = (Names.Length > 1 ? reader[Names[1]] : null);
                privateKey = (privateKey != DBNull.Value ? privateKey : null);

                mOriginalValue = new BlockchainKey((byte[])publicKey, (byte[])privateKey);
                mValue = mOriginalValue;
            }
            
            protected override object GetValueObject(ref object memberValue, int index) {
                if ((index < 0) || (index > 1))
                    throw new IndexOutOfRangeException();

                if (memberValue != DBNull.Value) {
                    BlockchainKey value = GetValue(ref memberValue);
                    if (value == null)
                        return null;
                    else if (index == 0)
                        return value.Public;
                    else
                        return value.Private;
                }
                else
                    return DBNull.Value;
            }
            
            public override bool IsNull { get { return ((mValue != null) ); } }
        }

        //********************************************************************************
        protected class DBBlockchainAddressField : DBField<BlockchainAddress> {
            public DBBlockchainAddressField(params string[] names) : base(names) { }

            public override void Load(SqlDataReader reader) {
                object value = reader[Name];
                if (value != DBNull.Value)
                    value = new BlockchainAddress((string)value);
                mOriginalValue = value;
                mValue = value;
            }
            
            protected override object GetValueObject(ref object memberValue, int index) {
                if (memberValue != DBNull.Value) {
                    BlockchainAddress value = GetValue(ref memberValue);
                    if (value == null)
                        return null;
                    else
                        return value.Address;
                }
                else
                    return DBNull.Value;
            }
        }

        //********************************************************************************
        protected class DBRSAKeyField : DBField<AsymmetricCipherKeyPair> {
            public DBRSAKeyField(params string[] names) : base(names) { }

            public override void Load(SqlDataReader reader) {
                object value = reader[Name];
                if (value != DBNull.Value)
                    value = RSA.KeyPairFromPEMString((string)value);
                mOriginalValue = value;
                mValue = value;
            }
            
            protected override object GetValueObject(ref object memberValue, int index) {
                if (memberValue != DBNull.Value) {
                    AsymmetricCipherKeyPair value = GetValue(ref memberValue);
                    if (value == null)
                        return null;
                    else
                        return RSA.KeyPairToPEMString(value);
                }
                else
                    return DBNull.Value;
            }
        }
    }

}
