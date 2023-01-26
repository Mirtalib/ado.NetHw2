using Microsoft.Extensions.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp8
{
    public partial class MainWindow : Window
    {
        SqlConnection? conn = null;
        IConfigurationRoot? builder = null;
        DataSet? dataset = null;
        SqlDataAdapter? adapter = null;
        string? providerName = null;


        public MainWindow()
        {
            InitializeComponent();

            DbProviderFactories.RegisterFactory("System.Data.SqlClient", typeof(SqlClientFactory));

            builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            providerName = "System.Data.SqlClient";
            conn = new(builder.GetConnectionString(providerName));
        }

    
        
        private void btnFill_Click(object sender, RoutedEventArgs e)
        {

            
            string SelectSqlCommand = "SELECT * FROM Authors";
            adapter = new SqlDataAdapter(SelectSqlCommand, conn);
            dataset = new DataSet();
            adapter.Fill(dataset);
            foreach (DataTable datatb in dataset.Tables)
            {
                DataGridView.ItemsSource = datatb.AsDataView();
            }
        }

        private void txtboxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (adapter is null)
                return;


            dataset?.Clear();
            
            string serach = txtboxSearch.Text;
            adapter.SelectCommand.CommandText = $"SELECT * FROM Authors WHERE UPPER(FirstName) LIKE UPPER('%{serach}%') OR UPPER(LastName) LIKE UPPER('%{serach}%')";

            adapter.Fill(dataset);
            foreach (DataTable item in dataset.Tables)
            {
                DataGridView.ItemsSource = item.AsDataView();
            }
            adapter.SelectCommand.CommandText = "SELECT * FROM Authors";
        }


        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand updateCommand = new SqlCommand()
            {
                CommandText = "usp_UpdateAuthors",
                Connection = conn,
                CommandType = CommandType.StoredProcedure,
            };


            updateCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
            updateCommand.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar));
            updateCommand.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar));


            updateCommand.Parameters["@Id"].SourceVersion = DataRowVersion.Current;
            updateCommand.Parameters["@Id"].SourceColumn = "Id";

            updateCommand.Parameters["@FirstName"].SourceVersion = DataRowVersion.Current;
            updateCommand.Parameters["@FirstName"].SourceColumn = "FirstName";

            updateCommand.Parameters["@LastName"].SourceVersion = DataRowVersion.Current;
            updateCommand.Parameters["@LastName"].SourceColumn = "LastName";


            adapter.UpdateCommand = updateCommand;
            adapter.Update(dataset);
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand InsertCommand = new SqlCommand("INSERT Authors VALUES(@Id,@FirstName,@LastName)", conn);
            InsertCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
            InsertCommand.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar));
            InsertCommand.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar));

            InsertCommand.Parameters["@Id"].SourceVersion = DataRowVersion.Current;
            InsertCommand.Parameters["@Id"].SourceColumn = "Id";

            InsertCommand.Parameters["@FirstName"].SourceVersion = DataRowVersion.Current;
            InsertCommand.Parameters["@FirstName"].SourceColumn = "FirstName";

            InsertCommand.Parameters["@LastName"].SourceVersion = DataRowVersion.Current;
            InsertCommand.Parameters["@LastName"].SourceColumn = "LastName";


            adapter.InsertCommand = InsertCommand;
            adapter.Update(dataset);
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand deleteCommand = new SqlCommand("DELETE Authors WHERE Id = @Id", conn);

            deleteCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
            deleteCommand.Parameters["@Id"].SourceVersion = DataRowVersion.Current;
            deleteCommand.Parameters["@Id"].SourceColumn = "Id";

            adapter.DeleteCommand = deleteCommand;
            adapter.Update(dataset);
        }
    }
}
