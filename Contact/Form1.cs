using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Contact
{
    public partial class Form1 : Form
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ContactManagementDB"].ConnectionString;

        public Form1()
        {
            InitializeComponent();
            LoadContacts();
            btnSearch.Click += btnSearch_Click; 
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadContacts();
        }

        private void LoadContacts()
        {
            listBoxContacts.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Contacts", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var contact = new Contact
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Email = reader["Email"].ToString()
                    };
                    listBoxContacts.Items.Add(contact);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO Contacts (Name, Phone, Email) VALUES (@Name, @Phone, @Email)", connection);
                command.Parameters.AddWithValue("@Name", txtName.Text);
                command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                command.Parameters.AddWithValue("@Email", txtEmail.Text);
                command.ExecuteNonQuery();
            }
            LoadContacts();
            ClearInputFields();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("UPDATE Contacts SET Name = @Name, Phone = @Phone, Email = @Email WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", selectedContact.Id);
                    command.Parameters.AddWithValue("@Name", txtName.Text);
                    command.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    command.Parameters.AddWithValue("@Email", txtEmail.Text);
                    command.ExecuteNonQuery();
                }
                LoadContacts();
                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите контакт для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("DELETE FROM Contacts WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", selectedContact.Id);
                    command.ExecuteNonQuery();
                }
                LoadContacts();
                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите контакт для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            listBoxContacts.Items.Clear();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Поиск по имени с использованием параметризованного запроса
                SqlCommand command = new SqlCommand(
                    "SELECT * FROM Contacts WHERE Name LIKE @SearchTerm", connection);
                command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%");

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var contact = new Contact
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Email = reader["Email"].ToString()
                    };
                    listBoxContacts.Items.Add(contact);
                }
            }
        }

        private void listBoxContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                txtName.Text = selectedContact.Name;
                txtPhone.Text = selectedContact.Phone;
                txtEmail.Text = selectedContact.Email;
            }
        }

        private void ClearInputFields()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtSearch.Clear(); // Очистка поля поиска
            listBoxContacts.ClearSelected();
        }
    }
}


