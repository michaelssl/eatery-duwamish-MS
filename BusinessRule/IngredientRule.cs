using Common.Data;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemFramework;

namespace BusinessRule
{
    public class IngredientRule
    {

        public int InsertUpdateIngredient(IngredientData ingredient)
        {
            SqlConnection SqlConn = null;
            SqlTransaction SqlTran = null;

            try
            {
                SqlConn = new SqlConnection(SystemConfigurations.EateryConnectionString);
                SqlConn.Open();
                SqlTran = SqlConn.BeginTransaction();
                int rowAffected = new IngredientDB().InsertUpdateIngredient(ingredient, SqlTran);
                SqlTran.Commit();
                SqlConn.Close();
                return rowAffected;

            }
            catch (Exception ex)
            {
                SqlTran.Rollback();
                SqlConn.Close();
                throw ex;
            }

        }

    }
}
