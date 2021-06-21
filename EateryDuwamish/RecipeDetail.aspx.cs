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
    public partial class RecipeDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request["id"] == null)
            {
                Response.Redirect("Dish.aspx");
            }

            int RecipeID = Convert.ToInt32(Request["id"]);
            RecipeData recipe = new RecipeSystem().GetRecipeByID(RecipeID);
            if (recipe == null)
            {
                Response.Redirect("Dish.aspx");
            }

            if (!IsPostBack)
            {
                ShowNotificationIfExists();
                LoadIngredientTable(RecipeID);
                lblRecipeName.Text = recipe.RecipeName.ToString();
                txtRecipeDescription.Text = recipe.RecipeDescription.ToString();
            }


        }


        #region FORM MANAGEMENT
        private void FillForm(IngredientData ingredient)
        {
            hdfIngredientId.Value = ingredient.IngredientID.ToString();
            hdfRecipeId.Value = ingredient.RecipeID.ToString();
            txtIngredientName.Text = ingredient.IngredientName.ToString();
            txtQuantity.Text = ingredient.IngredientQty.ToString();
            txtUnit.Text = ingredient.IngredientUnit.ToString();
        }

        private void ResetForm()
        {
            hdfIngredientId.Value = String.Empty;
            hdfRecipeId.Value = String.Empty;
            txtIngredientName.Text = String.Empty;
            txtQuantity.Text = String.Empty;
            txtUnit.Text = String.Empty;
        }

        private IngredientData GetFormData()
        {
            IngredientData ingredient = new IngredientData();
            ingredient.IngredientID = String.IsNullOrEmpty(hdfIngredientId.Value) ? 0 : Convert.ToInt32(hdfIngredientId.Value);
            ingredient.RecipeID = Convert.ToInt32(hdfRecipeId.Value);
            ingredient.IngredientName = Convert.ToString(txtIngredientName.Text);
            ingredient.IngredientQty = Convert.ToInt32(txtQuantity.Text);
            ingredient.IngredientUnit = Convert.ToString(txtUnit.Text);

            return ingredient;
        }

        private RecipeData GetFormDescriptionData()
        {
            RecipeData recipe = new RecipeData();
            recipe.RecipeID = Convert.ToInt32(Request["id"]);
            recipe.RecipeDescription = Convert.ToString(txtRecipeDescription.Text);
            return recipe;
        }

        #endregion

        #region DATA TABLE MANAGEMENT
        private void LoadIngredientTable(int RecipeID)
        {
            try
            {
                List<IngredientData> ListIngredient = new IngredientSystem().GetListIngredientByRecipeID(RecipeID);
                rptIngredient.DataSource = ListIngredient;
                rptIngredient.DataBind();

            }
            catch (Exception ex)
            {
                notifRecipeDetail.Show($"ERROR LOAD TABLE: {ex.Message}", NotificationType.Danger);
            }
        }

        protected void rptDish_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                IngredientData ingredient = (IngredientData)e.Item.DataItem;
                Literal litIngredientName = (Literal)e.Item.FindControl("litIngredientName");
                Literal litQuantity = (Literal)e.Item.FindControl("litQuantity");
                Literal litUnit = (Literal)e.Item.FindControl("litUnit");
                LinkButton litToggle = (LinkButton)e.Item.FindControl("lbToggle");

                litIngredientName.Text = ingredient.IngredientName;
                litQuantity.Text = ingredient.IngredientQty.ToString();
                litUnit.Text = ingredient.IngredientUnit;

                litToggle.CommandArgument = ingredient.IngredientID.ToString();

                CheckBox chkChoose = (CheckBox)e.Item.FindControl("chkChoose");
                chkChoose.Attributes.Add("data-value", ingredient.IngredientID.ToString());

            }

        }

        protected void rptDish_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if(e.CommandName == "EDIT")
            {
                Literal litIngredientName = (Literal)e.Item.FindControl("litIngredientName");

                int IngredientID = Convert.ToInt32(e.CommandArgument.ToString());
                IngredientData ingredient = new IngredientSystem().GetIngredientByID(IngredientID);
                FillForm(new IngredientData
                {
                    IngredientID = ingredient.IngredientID,
                    RecipeID = ingredient.RecipeID,
                    IngredientName = ingredient.IngredientName,
                    IngredientQty = ingredient.IngredientQty,
                    IngredientUnit = ingredient.IngredientUnit,
                     
                });

                litFormType.Text = $"UBAH: {litIngredientName.Text}";
                pnlFormIngredient.Visible = true;
                txtIngredientName.Focus();

            }
        }

        #endregion

        #region BUTTON EVENT MANAGEMENT
        protected void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                IngredientData ingredient = GetFormData();
                int rowAffected = new IngredientSystem().InsertUpdateIngredient(ingredient);
                if (rowAffected <= 0)
                    throw new Exception("No Data Recorded");
                Session["save-success"] = 1;
                Response.Redirect($"RecipeDetail.aspx?id={ingredient.RecipeID}");

            }
            catch (Exception ex)
            {
                notifRecipeDetail.Show($"ERROR SAVE DATA: {ex.Message}", NotificationType.Danger);
            }


        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ResetForm();
            litFormType.Text = "TAMBAH";

            int RecipeID = Convert.ToInt32(Request["id"]);
            hdfRecipeId.Value = RecipeID.ToString();

            pnlFormIngredient.Visible = true;
            txtIngredientName.Focus();

        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string strDeletedIDs = hdfDeletedIngredients.Value;
            IEnumerable<int> deletedIDs = strDeletedIDs.Split(',').Select(Int32.Parse);
            int rowAffected = new IngredientSystem().DeleteIngredients(deletedIDs);
            if (rowAffected <= 0)
                throw new Exception("No Data Deleted");
            Session["delete-success"] = 1;
            Response.Redirect($"RecipeDetail.aspx?id={Convert.ToInt32(Request["id"])}");
            
        }

        protected void btnEditDescription_Click(object sender, EventArgs e)
        {

            if (txtRecipeDescription.ReadOnly)
            {
                txtRecipeDescription.ReadOnly = false;
                return;
            }

            //txtRecipeDescription.ReadOnly = true;

        }

        protected void btnSaveDescription_Click(object sender, EventArgs e)
        {
            try
            {
                if (!txtRecipeDescription.ReadOnly)
                {
                    RecipeData recipe = GetFormDescriptionData();
                    int rowAffected = new RecipeSystem().UpdateRecipeDescription(recipe);
                    if(rowAffected <=0)
                        throw new Exception("No Data Recorded");
                    Session["save-success"] = 1;
                    Response.Redirect($"RecipeDetail.aspx?id={recipe.RecipeID}");
                }


            }
            catch (Exception ex)
            {
                notifRecipeDetail.Show($"ERROR SAVE DATA: {ex.Message}", NotificationType.Danger);
            }
        }
        #endregion

        #region NOTIFICATION MANAGEMENT
        private void ShowNotificationIfExists()
        {
            if (Session["save-success"] != null)
            {
                notifRecipeDetail.Show("Data sukses disimpan", NotificationType.Success);
                Session.Remove("save-success");
            }
            if (Session["delete-success"] != null)
            {
                notifRecipeDetail.Show("Data sukses dihapus", NotificationType.Success);
                Session.Remove("delete-success");
            }
        }

        #endregion

    }
}