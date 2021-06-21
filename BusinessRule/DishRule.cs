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
    public class DishRule
    {
        public int InsertUpdateDish(DishData dish)
        {
            SqlConnection SqlConn = null;
            SqlTransaction SqlTran = null;
            try
            {
                SqlConn = new SqlConnection(SystemConfigurations.EateryConnectionString);
                SqlConn.Open();
                SqlTran = SqlConn.BeginTransaction();
                int rowsAffected = new DishDB().InsertUpdateDish(dish, SqlTran);
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

        public int DeleteDishes(IEnumerable<int> dishIDs)
        {
            SqlConnection SqlConn = null;
            SqlTransaction SqlTran = null;
            try
            {
                SqlConn = new SqlConnection(SystemConfigurations.EateryConnectionString);
                SqlConn.Open();
                SqlTran = SqlConn.BeginTransaction();

                // Ambil Semua Recipe dari tiap-tiap Dish
                foreach (int dishID in dishIDs)
                {
                    string strRecipeIDs = "";
                    List<RecipeData> listRecipe = new RecipeDB().GetListRecipeByDishID(dishID);

                    // Ambil Semua Ingredient dari tiap-tiap Recipe
                    foreach (RecipeData recipe in listRecipe)
                    {
                        string strIngredientIDs = "";
                        List<IngredientData> listIngredient = new IngredientDB().GetListIngredientByRecipeID(recipe.RecipeID);
                        foreach (IngredientData ingredient in listIngredient)
                        {
                            strIngredientIDs += ingredient.IngredientID.ToString() + ',';
                        }
                        // Delete All Ingredient yang terhubung dengan recipe
                        int rowAffectedIngredient = new IngredientDB().DeleteIngredients(strIngredientIDs, SqlTran);

                        strRecipeIDs += recipe.RecipeID.ToString() + ',';
                    }

                    // Delete All Recipe yang terhubung dengan tiap-tiap dish yang di pilih
                    int rowAffectedRecipe = new RecipeDB().DeleteRecipes(strRecipeIDs, SqlTran);
                }
                // Delete All Dish yang dipilih
                int rowsAffectedDish = new DishDB().DeleteDishes(String.Join(",", dishIDs), SqlTran);

                SqlTran.Commit();
                SqlConn.Close();
                return rowsAffectedDish;
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
