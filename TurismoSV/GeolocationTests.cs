using System;
using System.Collections.Generic;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geolocation.Request;
using NUnit.Framework;
using System.Linq;
using System.Text;
using System.Threading;
using GMap.NET;

namespace TurismoSV
{
    [TestFixture]
    internal class GeolocationTests
    {
        public PointLatLng ubicacion;
        [Test]
        public void ObtenerUbicacion()
        {
            var request = new GeolocationRequest
            {
                Key = "AIzaSyC_NHObgFmos-g1kWRvwGgCPD2A0wbgoGQ"
            };

            var result = GoogleApi.GoogleMaps.Geolocation.Query(request);
            Assert.IsNotNull(result);
            Assert.AreEqual(Status.Ok, result.Status);
            ubicacion = new PointLatLng(result.Location.Latitude, result.Location.Longitude);
        }
    }
}
