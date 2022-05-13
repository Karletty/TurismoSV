using System;
using System.Collections.Generic;
using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geolocation.Request;
using GoogleApi.Entities.Places.Common.Enums;
using GoogleApi.Entities.Places.Search.Common.Enums;
using GoogleApi.Entities.Places.Search.NearBy.Request;
using NUnit.Framework;
using System.Linq;
using System.Text;
using System.Threading;
using GMap.NET;
using System.Windows.Forms;

namespace TurismoSV
{
    internal class GoogleApi
    {
        public PointLatLng ubicacion;
        public List<PointLatLng> lugares;
        public List<string> nombresLugares;
        public void ObtenerUbicacion()
        {
            var request = new GeolocationRequest
            {
                Key = AppConfig.Key
            };

            var result = GoogleMaps.Geolocation.Query(request);
            Assert.IsNotNull(result);
            Assert.AreEqual(Status.Ok, result.Status);
            ubicacion = new PointLatLng(result.Location.Latitude, result.Location.Longitude);
        }

        public void PlacesTextSearchKeyword(string keyWord, Double lat, Double lng, Double radius)
        {
            lugares = new List<PointLatLng>();
            nombresLugares = new List<string>();
            var request = new PlacesNearBySearchRequest
            {
                Key = AppConfig.Key,
                Location = new Coordinate(lat, lng),
                Radius = radius,
                Keyword = keyWord
            };
            var response = GooglePlaces.NearBySearch.Query(request);
            Assert.IsNotNull(response);
            if (response.Results.Count() > 0)
            {
                Assert.AreEqual(Status.Ok, response.Status);
                for (int i = 0; i < response.Results.Count(); i++)
                {
                    Coordinate result = response.Results.ElementAt(i).Geometry.Location;
                    string nombre = response.Results.ElementAt(i).Name;
                    Double l = result.Latitude, ln = result.Longitude;
                    lugares.Add(new PointLatLng(l, ln));
                    nombresLugares.Add(nombre);
                }
                MessageBox.Show($"Se han encontrado {response.Results.Count()} lugares del tipo {keyWord} en el radio de {radius / 1000} km");
            }
            else
            {
                MessageBox.Show($"No se han encontrado lugares del tipo {keyWord} en el radio de {radius / 1000} km");
            }
        }
    }
}
