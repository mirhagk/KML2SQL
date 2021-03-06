﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using Microsoft.SqlServer.Types;

namespace KML2SQL
{
    class MapFeature
    {
        readonly Placemark _placemark;

        public int Id;
        public string Name {get { return _placemark.Name ?? Id.ToString(); }}
        public Vector[] Coordinates { get; private set; }
        public Dictionary<string, string> Data = new Dictionary<string, string>();

        public OpenGisGeographyType? GeographyType { get; private set; }
        public OpenGisGeometryType? GeometryType { get; private set; }

        public MapFeature(Placemark placemark, int id)
        {
            _placemark = placemark;
            Id = id;
            SetGeoTypes(placemark);
            InitializeCoordinates(placemark);
            InitializeData(placemark);
        }

        private void SetGeoTypes(Placemark placemark)
        {
            foreach (var element in placemark.Flatten())
            {
                if (element is Point)
                {
                    GeographyType = OpenGisGeographyType.Point;
                    GeometryType = OpenGisGeometryType.Point;
                }
                else if (element is Polygon)
                {
                    GeographyType = OpenGisGeographyType.Polygon;
                    GeometryType = OpenGisGeometryType.Polygon;
                }
                else if (element is LineString)
                {
                    GeographyType = OpenGisGeographyType.LineString;
                    GeometryType = OpenGisGeometryType.LineString;
                }
            }
        }


        private void InitializeCoordinates(Placemark placemark)
        {
            switch (this.GeometryType)
            {
                case OpenGisGeometryType.LineString:
                    Coordinates = InitializeLineCoordinates(placemark);
                    break;
                case OpenGisGeometryType.Point:
                    Coordinates = InitializePointCoordinates(placemark);
                    break;
                case OpenGisGeometryType.Polygon:
                    Coordinates = InitializePolygonCoordinates(placemark);
                    break;
            }
        }

        private void InitializeData(Placemark placemark)
        {
            foreach (SimpleData sd in placemark.Flatten().OfType<SimpleData>())
            {
                if (sd.Name.ToLower() == "id")
                {
                    sd.Name = "sd_id";
                }
                Data.Add(sd.Name.Sanitize(), sd.Text.Sanitize());
            }
            foreach (Data data in placemark.Flatten().OfType<Data>())
            {
                if (data.Name.ToLower() == "id")
                {
                    data.Name = "data_id";
                }
                Data.Add(data.Name.Sanitize(), data.Value.Sanitize());
            }
        }

        private static Vector[] InitializePointCoordinates(Placemark placemark)
        {
            List<Vector> coordinates = new List<Vector>();
            foreach (var point in placemark.Flatten().OfType<Point>())
            {
                Vector myVector = new Vector();
                myVector.Latitude = point.Coordinate.Latitude;
                myVector.Longitude = point.Coordinate.Longitude;
                coordinates.Add(myVector);
            }
            return coordinates.ToArray();
        }

        private static Vector[] InitializeLineCoordinates(Placemark placemark)
        {
            List<Vector> coordinates = new List<Vector>();
            foreach (LineString element in placemark.Flatten().OfType<LineString>())
            {
                LineString lineString = element;
                coordinates.AddRange(lineString.Coordinates);
            }
            return coordinates.ToArray();
        }

        private static Vector[] InitializePolygonCoordinates(Placemark placemark)
        {
            List<Vector> coordinates = new List<Vector>();
            foreach (var polygon in placemark.Flatten().OfType<Polygon>())
            {
                coordinates.AddRange(polygon.OuterBoundary.LinearRing.Coordinates);
            }
            return coordinates.ToArray();
        }

        public void ReverseRingOrientation()
        {
            List<Vector> reversedCoordinates = new List<Vector>();
            for (int i = Coordinates.Length - 1; i >= 0; i--)
            {
                reversedCoordinates.Add(Coordinates[i]);
            }
            Coordinates = reversedCoordinates.ToArray();
        }

        public override string ToString()
        {
            return Name + " " + Id + " - " + GeometryType;
        }
    }
}
