using BusinessFacade;
using Common.Data;
using Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EateryDuwamish
{
    public partial class Recipe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request["id"] == null)
            {
                Response.Redirect("Dish.aspx");
            }

            int DishID = Convert.ToInt32(Request["id"]);
            DishData dish = new DishSystem().GetDishByID(DishID);

            if(dish == null)
            {
                Response.Redirect("Dish.aspx");
            }


            if (!IsPostBack)
            {
                ShowNotificationIfExists();
                LoadRecipeTable(DishID);

                lblDishName.Text = dish.DishName;
                DishTypeData dishType = new DishTypeSystem().GetDishTypeByID(dish.DishTypeID);
                lblDishType.Text = dishType.DishTypeName;
            }

        }

        #region FORM MANAGEMENT
        private void FillForm(RecipeData recipe)
        {
            hdfRecipeId.Value = recipe.RecipeID.ToString();
            hdfDishId.Value = recipe.DishID.ToString();
            txtRecipeName.Text = recipe.RecipeName.ToString();
        }

        private void ResetForm()
        {
            hdfRecipeId.Value = String.Empty;
            hdfDishId.Value = String.Empty;
            txtRecipeName.Text = String.Empty;
        }

        private RecipeData GetFormData()
        {
            RecipeData recipe = new RecipeData();
            recipe.RecipeID = String.IsNullOrEmpty(hdfRecipeId.Value) ? 0 : Convert.ToInt32(hdfRecipeId.Value);
            recipe.DishID = Convert.ToInt32(hdfDishId.Value);
            recipe.RecipeName = txtRecipeName.Text;
            return recipe;
        }

        #endregion

        #region DATA TABLE MANAGEMENT
        private void LoadRecipeTable(int DishID)
        {
            try
            {
                List<RecipeData> ListRecipe = new RecipeSystem().GetListRecipeByDishID(DishID);
                rptRecipe.DataSource = ListRecipe;
                rptRecipe.DataBind();
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR LOAD TABLE: {ex.Message}", NotificationType.Danger);
            }
        }

        protected void rptRecipe_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RecipeData recipe = (RecipeData)e.Item.DataItem;
                LinkButton lbRecipeName = (LinkButton)e.Item.FindControl("lbRecipeName");
                HyperLink hplIngredient = (HyperLink)e.Item.FindControl("hplIngredient");

                lbRecipeName.Text = recipe.RecipeName;
                lbRecipeName.CommandArgument = recipe.RecipeID.ToString();

                CheckBox chkChoose = (CheckBox)e.Item.FindControl("chkChoose");
                chkChoose.Attributes.Add("data-value", recipe.RecipeID.ToString());

                hplIngredient.NavigateUrl = $"~/RecipeDetail.aspx?id={recipe.RecipeID.ToString()}";

            }
        }

        protected void rptRecipe_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if(e.CommandName == "EDIT")
            {
                LinkButton lbRecipeName = (LinkButton)e.Item.FindControl("lbRecipeName");

                int RecipeID = Convert.ToInt32(e.CommandArgument.ToString());
                RecipeData recipe = new RecipeSystem().GetRecipeByID(RecipeID);
                FillForm(new RecipeData
                {
                    RecipeID = recipe.RecipeID,
                    DishID = recipe.DishID,
                    RecipeName = recipe.RecipeName,
                });

                litFormType.Text = $"UBAH: {lbRecipeName.Text}";
                pnlFormRecipe.Visible = true;
                txtRecipeName.Focus();

            }

        }

        #endregion

        #region BUTTON EVENT MANAGEMENT
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                RecipeData recipe = GetFormData();
                int rowAffected = new RecipeSystem().InsertUpdateRecipe(recipe);
                if (rowAffected <= 0)
                    throw new Exception("No Data Recorded");
                Session["save-success"] = 1;
                Response.Redirect($"Recipe.aspx?id={recipe.DishID}");
            }
            catch (Exception ex)
            {
                notifRecipe.Show($"ERROR SAVE DATA: {ex.Message}", NotificationType.Danger);
            }


        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ResetForm();
            litFormType.Text = "TAMBAH";

            int DishID = Convert.ToInt32(Request["id"]);
            hdfDishId.Value = DishID.ToString();

            pnlFormRecipe.Visible = true;
            txtRecipeName.Focus();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string strDeletedIDs = hdfDeletedRecipes.Value;
            IEnumerable<int> deletedIDs = strDeletedIDs.Split(',').Select(Int32.Parse);
            int rowAffected = new RecipeSystem().DeleteRecipes(deletedIDs);
            if (rowAffected <= 0)
                throw new Exception("No Data Deleted");
            Session["delete-success"] = 1;
            Response.Redirect($"Recipe.aspx?id={Convert.ToInt32(Request["id"])}");
       
        }

        #endregion

        #region NOTIFICATION MANAGEMENT
        private void ShowNotificationIfExists()
        {
            if (Session["save-success"] != null)
            {
                notifRecipe.Show("Data sukses disimpan", NotificationType.Success);
                Session.Remove("save-success");
            }
            if (Session["delete-success"] != null)
            {
                notifRecipe.Show("Data sukses dihapus", NotificationType.Success);
                Session.Remove("delete-success");
            }
        }
        #endregion


    }
}