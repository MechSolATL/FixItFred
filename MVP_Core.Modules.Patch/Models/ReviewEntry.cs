using System;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Models;

namespace MVP_Core.Modules.Patch.Models
{
    public class ReviewEntry
    {
        public int Id { get; set; }
        public string CustomerFullName { get; set; }
        public string TechnicianName { get; set; }
        public string Platform { get; set; }
        public string ReviewText { get; set; }
        public int StarRating { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string ServiceCategory { get; set; }

        [NotMapped]
        public string GoogleMapsUrl =>
            $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(StreetAddress + ", " + City + ", " + State + " " + ZipCode)}";

        public ReviewResponseLog ResponseLog { get; set; }

        public string GetDisplayAddress(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => $"{StreetAddress}, {City}, {State} {ZipCode}",
                UserRole.Technician => $"{StreetAddress.Split(' ')[0]} St, {City} {State}",
                _ => $"{City}, {State} {ZipCode}"
            };
        }
    }
}