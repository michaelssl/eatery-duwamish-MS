using BusinessRule;
using Common.Data;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessFacade
{
    public class IngredientSystem
    {

        public List<IngredientData> GetListIngredientByRecipeID(int RecipeID)
        {
            try
            {
                return new IngredientDB().GetListIngredientByRecipeID(RecipeID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IngredientData GetIngredientByID(int IngredientID)
        {
            try
            {
                return new IngredientDB().GetIngredientByID(IngredientID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int InsertUpdateIngredient(IngredientData ingredient)
        {
            try
            {
                return new IngredientRule().InsertUpdateIngredient(ingredient);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
