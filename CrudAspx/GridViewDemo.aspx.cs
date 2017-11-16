using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CrudAspx
{
    public partial class GridViewDemo : System.Web.UI.Page
    {

        private string strConnectionString = ConfigurationManager.ConnectionStrings["myconnection"].ConnectionString;
        private SqlCommand _sqlCommand;
        private SqlDataAdapter _sqlDataAdapter;
        DataSet _dtSet;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEmployeeData();

            }
            btnUpdate.Visible = false;
            btnAddEmployee.Visible = true;
        }



        private static void ShowAlertMessage(string error)
        {
            System.Web.UI.Page page = System.Web.HttpContext.Current.Handler as System.Web.UI.Page;
            if (page != null)
            {
                error = error.Replace("'", "\'");
                System.Web.UI.ScriptManager.RegisterStartupScript(page, page.GetType(), "err_msg", "alert('" + error + "');", true);
            }
        }
        public void CreateConnection()
        {
            SqlConnection _sqlConnection = new SqlConnection(strConnectionString);
            _sqlCommand = new SqlCommand();
            _sqlCommand.Connection = _sqlConnection;
        }
        public void OpenConnection()
        {
            _sqlCommand.Connection.Open();
        }
        public void CloseConnection()
        {
            _sqlCommand.Connection.Close();
        }
        public void DisposeConnection()
        {
            _sqlCommand.Connection.Dispose();
        }
        public void BindEmployeeData()
        {
            try
            {
                CreateConnection();
                OpenConnection();
                _sqlCommand.CommandText = "Sp_GridCrud";
                _sqlCommand.CommandType = CommandType.StoredProcedure;
                _sqlCommand.Parameters.AddWithValue("@Event", "Select");
                _sqlDataAdapter = new SqlDataAdapter(_sqlCommand);
                _dtSet = new DataSet();
                _sqlDataAdapter.Fill(_dtSet);
                grvEmployee.DataSource = _dtSet;
                grvEmployee.DataBind();
            }
            catch (Exception ex)
            {
                Response.Redirect("The Error is " + ex);
            }
            finally
            {
                CloseConnection();
                DisposeConnection();
            }
        }

        protected void btnAddEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                CreateConnection();
                OpenConnection();
                _sqlCommand.CommandText = "Sp_GridCrud";
                _sqlCommand.CommandType = CommandType.StoredProcedure;
                _sqlCommand.Parameters.AddWithValue("@Event", "Add");
                _sqlCommand.Parameters.AddWithValue("@FirstName", Convert.ToString(txtFirstName.Text.Trim()));
                _sqlCommand.Parameters.AddWithValue("@LastName", Convert.ToString(txtLastName.Text.Trim()));
                _sqlCommand.Parameters.AddWithValue("@PhoneNumber", Convert.ToString(txtPhoneNumber.Text.Trim()));
                _sqlCommand.Parameters.AddWithValue("@EmailAddress", Convert.ToString(txtEmailAddress.Text.Trim()));
                _sqlCommand.Parameters.AddWithValue("@Salary", Convert.ToDecimal(txtSalary.Text));
                int result = Convert.ToInt32(_sqlCommand.ExecuteNonQuery());
                if (result > 0)
                {

                    ShowAlertMessage("Record Is Inserted Successfully");
                    BindEmployeeData();
                    ClearControls();
                }
                else
                {

                    ShowAlertMessage("Failed");
                }
            }
            catch (Exception ex)
            {

                ShowAlertMessage("Check your input data");

            }
            finally
            {
                CloseConnection();
                DisposeConnection();
            }
        }

        public void ClearControls()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhoneNumber.Text = "";
            txtEmailAddress.Text = "";
            txtSalary.Text = "";
        }

        protected void grvEmployee_RowEditing(object sender, GridViewEditEventArgs e)
        {
            btnAddEmployee.Visible = false;
            btnUpdate.Visible = true;

            int RowIndex = e.NewEditIndex;
            Label empid = (Label)grvEmployee.Rows[RowIndex].FindControl("lblEmpId");
            Session["id"] = empid.Text;

            txtFirstName.Text = ((Label)grvEmployee.Rows[RowIndex].FindControl("lblFirstName")).Text.ToString();
            txtLastName.Text = ((Label)grvEmployee.Rows[RowIndex].FindControl("lblLastName")).Text.ToString();
            txtPhoneNumber.Text = ((Label)grvEmployee.Rows[RowIndex].FindControl("lblPhoneNumber")).Text.ToString();
            txtEmailAddress.Text = ((Label)grvEmployee.Rows[RowIndex].FindControl("lblEmailAddress")).Text.ToString();
            txtSalary.Text = ((Label)grvEmployee.Rows[RowIndex].FindControl("lblSalary")).Text.ToString();

        }

        protected void grvEmployee_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                CreateConnection();
                OpenConnection();
                Label id = (Label)grvEmployee.Rows[e.RowIndex].FindControl("lblEmpId");
                _sqlCommand.CommandText = "Sp_GridCrud";
                _sqlCommand.Parameters.AddWithValue("@Event", "Delete");
                _sqlCommand.Parameters.AddWithValue("@EmpId", Convert.ToInt32(id.Text));
                _sqlCommand.CommandType = CommandType.StoredProcedure;
                int result = Convert.ToInt32(_sqlCommand.ExecuteNonQuery());
                if (result > 0)
                {

                    ShowAlertMessage("Record Is Deleted Successfully");
                    grvEmployee.EditIndex = -1;
                    BindEmployeeData();
                }
                else
                {
                    lblMessage.Text = "Failed";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    BindEmployeeData();
                }
            }
            catch (Exception ex)
            {

                ShowAlertMessage("Check your input data");
            }
            finally
            {
                CloseConnection();
                DisposeConnection();
            }
        }

        protected void grvEmployee_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            grvEmployee.EditIndex = -1;
            BindEmployeeData();
        }

        protected void grvEmployee_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvEmployee.PageIndex = e.NewPageIndex;
            BindEmployeeData();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearControls();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                CreateConnection();
                OpenConnection();

                _sqlCommand.CommandText = "Sp_GridCrud";
                _sqlCommand.CommandType = CommandType.StoredProcedure;
                _sqlCommand.Parameters.AddWithValue("@Event", "Update");
                _sqlCommand.Parameters.AddWithValue("@FirstName", Convert.ToString(txtFirstName.Text.Trim()));
                _sqlCommand.Parameters.AddWithValue("@LastName", Convert.ToString(txtLastName.Text.Trim()));
                _sqlCommand.Parameters.AddWithValue("@PhoneNumber", Convert.ToString(txtPhoneNumber.Text.Trim()));
                _sqlCommand.Parameters.AddWithValue("@EmailAddress", Convert.ToString(txtEmailAddress.Text.Trim()));
                _sqlCommand.Parameters.AddWithValue("@Salary", Convert.ToDecimal(txtSalary.Text));
                _sqlCommand.Parameters.AddWithValue("@EmpId", Convert.ToDecimal(Session["id"]));

                int result = Convert.ToInt32(_sqlCommand.ExecuteNonQuery());
                if (result > 0)
                {
                    ShowAlertMessage("Record Is Updated Successfully");
                    grvEmployee.EditIndex = -1;
                    BindEmployeeData();
                    ClearControls();
                }
                else
                {
                    ShowAlertMessage("Failed");
                }
            }

            catch (Exception ex)
            {
                ShowAlertMessage("Check your input data");
            }
            finally
            {
                CloseConnection();
                DisposeConnection();
            }
        }


    }
}