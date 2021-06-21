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
    public class RecipeRule
    {

        public int InsertUpdateRecipe(RecipeData recipe)
        {
            SqlConnection SqlConn = null;
            SqlTransaction SqlTran = null;

            try
            {
                SqlConn = new SqlConnection(SystemConfigurations.EateryConnectionString);
                SqlConn.Open();
                SqlTran = SqlConn.BeginTransaction();
                int rowsAffected = new RecipeDB().InsertUpdateRecipe(recipe, SqlTran);
                SqlTran.Commit();
                SqlConn.Close();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                SqlTran.Rollback();
                SqlConn.Close();
                throw ex;
            }
        }

        public int UpdateRecipeDescription(RecipeData recipe)
        {
            SqlConnection SqlConn = null;
            SqlTransaction SqlTran = null;

            try
            {
                SqlConn = new SqlConnection(SystemConfigurations.EateryConnectionString);
                SqlConn.Open();
                SqlTran = SqlConn.BeginTransaction();
                int rowsAffected = new RecipeDB().UpdateRecipeDescription(recipe, SqlTran);
                SqlTran.Commit();
                SqlConn.Close();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                SqlTran.Rollback();
                SqlConn.Close();
                throw ex;
            }
        }

        public int DeleteRecipes(IEnumerable<int> recipeIDs)
        {

            SqlConnection SqlConn = null;
            SqlTransaction SqlTran = null;

            try
            {
                SqlConn = new SqlConnection(SystemConfigurations.EateryConnectionString);
                SqlConn.Open();
                SqlTran = SqlConn.BeginTransaction();

                // Ambil semua ingredient dari semua recipe yang ingin di delete
                foreach (int recipeID in recipeIDs)
                {
                    string strIngredientIDs="";

                    List<IngredientData> listIngredient = new IngredientDB().GetListIngredientByRecipeID(recipeID);
                    foreach (IngredientData ingredient in listIngredient)
                    {
                        strIngredientIDs += ingredient.IngredientID.ToString() + ',';
                    }
                    // Delete semua ingredient dari semua recipe yang ingin di delete
                    int rowsAffectedIngredient = new IngredientDB().DeleteIngredients(strIngredientIDs, SqlTran);
                }

                // Delete All Recipe yang dipilih
                int rowAffectedRecipe = new RecipeDB().DeleteRecipes(String.Join(",", recipeIDs), SqlTran);

                SqlTran.Commit();
                SqlConn.Close();
                return rowAffectedRecipe;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


    }
}
