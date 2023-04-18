using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using WebAppTextEditor.Models;

namespace WebAppTextEditor.Controllers
{
    public class EditorController : Controller
    {
        IConfiguration _configuration;
        SqlConnection _connection;


        public EditorController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("EditorDB"));

        }

        public List<EditorModel> _EditorList = new List<EditorModel>();
        public IActionResult Index()
        {
            _connection.Open();
            string selectQuery = "SELECT * FROM dbo.TextEditorTable";
            using (SqlCommand cmd = new SqlCommand(selectQuery, _connection))
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    EditorModel editor = new EditorModel();


                    editor.DocID = (int)reader[0];
                    editor.ContentDetails = (string)reader[1];
                    _EditorList.Add(editor);
                }
                reader.Close();
            }

            ViewBag.EditorList = _EditorList;
            return View(ViewBag);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EditorModel editor)
        {
            _connection.Open();

            editor.ContentDetails = Request.Form["ContentDetails"];



            string addScheduleQuery = $"INSERT INTO TextEditorTable VALUES('{editor.ContentDetails}')";

            try
            {
                using (SqlCommand cmd = new SqlCommand(addScheduleQuery, _connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("Index");
        }

        public EditorModel GetDocument(int id)

        {
            EditorModel editor = new EditorModel();
            _connection.Open();
            SqlCommand cmd = _connection.CreateCommand();

            string query = $"SELECT * FROM dbo.TextEditorTable where DocID={id}";
            cmd.CommandText = query;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                editor.ContentDetails = (string)reader["ContentDetails"];
            }
            reader.Close();
            _connection.Close();
            //appointmentList = db.AppointmentDetails.Where(x=>x.email==current_user_email).ToList();
            return editor;
        }

        public IActionResult Edit(int id)
        {
            return View(GetDocument(id));
        }

        [HttpPost]
        public IActionResult Edit(int id, EditorModel editor)
        {
            try
            {
                _connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE_TEXTEDITOR", _connection);
                cmd.CommandType = CommandType.StoredProcedure;

                DateTime current_date = DateTime.Now;
                cmd.Parameters.AddWithValue("@DocID", id);
                cmd.Parameters.AddWithValue("@ContentDetails", editor.ContentDetails);

                cmd.ExecuteNonQuery();

                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction("Index", "Editor");
        }


        public IActionResult Delete(int id)
        {
            return View(GetDocument(id));
        }
        [HttpDelete]
        public IActionResult Delete(int id, EditorModel editor)
        {
            try
            {
                _connection.Open();
                SqlCommand cmd = new SqlCommand("Delete_TextEditor", _connection);
                cmd.CommandType = CommandType.StoredProcedure;
                DateTime current_date = DateTime.Now;
                cmd.Parameters.AddWithValue("@DocID", id);
                cmd.Parameters.AddWithValue("@ContentDetails", editor.ContentDetails);
                cmd.ExecuteNonQuery();

                _connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction("Index", "Editor");
        }
        public IActionResult View(int id)
        {
            //return RedirectToAction("Index");
            return View(GetDocument(id));
        }
    }
}
