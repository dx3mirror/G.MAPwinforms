using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System.Linq;
namespace GMap.NET_HHRU
{
    public partial class Start : Form
    {
        private GMapOverlay markersOverlay;
        private SqlConnection connection;

        public Start()
        {
            InitializeComponent();
            InitializeMap();
            InitializeDatabaseConnection();
            LoadMarkersFromDatabase();

        }
        private void InitializeMap()
        {
            // Установка провайдера карты и начальных координат
            gmap.MapProvider = GoogleMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gmap.Position = new PointLatLng(55.7522, 37.6156); // Начальные координаты (Москва)
            gmap.Zoom = 10; // Уровень масштабирования

            // Создание слоя для маркеров
            markersOverlay = new GMapOverlay("markers");
            gmap.Overlays.Add(markersOverlay);
        }

        private void InitializeDatabaseConnection()
        {
            string connectionString = "Data Source=DESKTOP-BM7NK88;Initial Catalog=Map;Integrated Security=True";
            connection = new SqlConnection(connectionString);
        }

        private void LoadMarkersFromDatabase()
        {
            try
            {
                connection.Open();
                string query = "SELECT Latitude, Longitude FROM MAPKOORDINAT";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    double latitude = Convert.ToDouble(reader["Latitude"]);
                    double longitude = Convert.ToDouble(reader["Longitude"]);
                    PointLatLng point = new PointLatLng(latitude, longitude);
                    GMarkerGoogle marker = new GMarkerGoogle(point, GMarkerGoogleType.red);
                    markersOverlay.Markers.Add(marker);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке маркеров из базы данных: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void gmap_OnMarkerEnter(GMapMarker item)
        {
            // Отслеживание события наведения курсора на маркер
            gmap.Cursor = Cursors.Hand;
        }

        private void gmap_OnMarkerLeave(GMapMarker item)
        {
            // Отслеживание события ухода курсора с маркера
            gmap.Cursor = Cursors.Default;
        }

        private void gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            // Отслеживание события клика на маркере
            if (e.Button == MouseButtons.Left)
            {
                item.IsVisible = false; // Скрытие маркера
                gmap.Cursor = Cursors.SizeAll; // Изменение курсора
            }
        }

        private void gmap_MouseMove(object sender, MouseEventArgs e)
        {
            // Отслеживание события перемещения мыши по карте
            if (e.Button == MouseButtons.Left && gmap.Cursor == Cursors.SizeAll)
            {
                PointLatLng point = gmap.FromLocalToLatLng(e.X, e.Y); // Получение координат точки на карте
                GMarkerGoogle marker = (GMarkerGoogle)markersOverlay.Markers.FirstOrDefault(m => !m.IsVisible); // Поиск скрытого маркера

                if (marker != null)
                {
                    marker.Position = point; // Изменение позиции маркера
                }
            }
        }

        private void gmap_MouseUp(object sender, MouseEventArgs e)
        {
            // Отслеживание события отпускания кнопки мыши на карте
            if (e.Button == MouseButtons.Left && gmap.Cursor == Cursors.SizeAll)
            {
                gmap.Cursor = Cursors.Default;// Возврат стандартного курсора
                GMarkerGoogle marker = (GMarkerGoogle)markersOverlay.Markers.FirstOrDefault(m => !m.IsVisible); // Поиск скрытого маркера

                if (marker != null)
                {
                    marker.IsVisible = true; // Отображение маркера

                    try
                    {
                        connection.Open();
                        string query = "UPDATE MAPKOORDINAT SET Latitude = @Latitude, Longitude = @Longitude WHERE Id = @Id";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Latitude", marker.Position.Lat);
                        command.Parameters.AddWithValue("@Longitude", marker.Position.Lng);
                        command.Parameters.AddWithValue("@Id", marker.Tag); // Предполагается, что у маркера есть уникальный идентификатор в базе данных
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении координат маркера: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void Start_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Отслеживание события закрытия формы
            try
            {
                connection.Open();

                foreach (GMarkerGoogle marker in markersOverlay.Markers)
                {
                    string query = "UPDATE MAPKOORDINAT SET Latitude = @Latitude, Longitude = @Longitude WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Latitude", marker.Position.Lat);
                    command.Parameters.AddWithValue("@Longitude", marker.Position.Lng);
                    command.Parameters.AddWithValue("@Id", marker.Tag); // Предполагается, что у маркера есть уникальный идентификатор в базе данных
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении координат маркеров: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
    

       
        
