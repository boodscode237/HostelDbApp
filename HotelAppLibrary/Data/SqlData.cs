using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelAppLibrary.Database;
using HotelAppLibrary.Models;

namespace HotelAppLibrary.Data
{
    public class SqlData : IDatabaseData
    {
        private readonly ISqlDataAccess _db;
        private const string ConnectionStringName = "SqlDb";

        public SqlData(ISqlDataAccess db)
        {
            _db = db;

        }

        public List<RoomTypeModel> GetAvailableRoomTypes(DateTime startDate, DateTime endDate)
        {
            return _db.LoadData<RoomTypeModel, dynamic>(
                "dbo.spRoomTypes_GetAvailableTypes",
                new { startDate, endDate },
                ConnectionStringName,
                true);
        }

        public void BookGuest(string firstName,
            string lastName,
            DateTime startDate,
            DateTime endDate,
            int roomTypeId)
        {
            GuestModel guest = _db
                .LoadData<GuestModel, dynamic>(
                    "dbo.spGuests_Insert", 
                    new
                    {
                        firstName,
                        lastName
                    }, 
                    ConnectionStringName, 
                    true).First();

            RoomTypeModel roomType = _db.LoadData<RoomTypeModel, dynamic>(
                "select * from dbo.RoomTypes where Id = @Id",
                new {Id = roomTypeId},
                ConnectionStringName,
                    false).First();

            TimeSpan timeStaying = endDate.Date.Subtract(startDate.Date);
            // timeStaying.Days;
            
            List<RoomModel> availableRooms = _db.LoadData<RoomModel, dynamic>(
                "spRooms_GetAvailableRooms",
                new { startDate, endDate, roomTypeId},
                ConnectionStringName,
                true);

            _db.SaveData(
                "spBookings_Insert",
                new
                {
                    roomId = availableRooms.First().Id,
                    guestId = guest.Id,
                    startDate = startDate,
                    endDate = endDate,
                    totalCost = timeStaying.Days * roomType.Price
                },
                ConnectionStringName,
                true);

        }

        public List<BookingFullModel> SearchBookings(string lastName)
        {
            return _db.LoadData<BookingFullModel, dynamic>(
                "dbo.spBookings_Search",
                new { lastName, startDate = DateTime.Now.Date},
                ConnectionStringName,
                true);
        }

        public void CheckInGuest(int bookingId)
        {
            _db.SaveData(
                "dbo.spBookings_CheckIn",
                new{Id = bookingId},
                ConnectionStringName,
                true);
        }

        public RoomTypeModel GetRoomTypeById(int id)
        {
            return _db.LoadData<RoomTypeModel, dynamic>(
                "dbo.spRoomTypes_GetById",
                new {id},
                ConnectionStringName,
                true).FirstOrDefault();
        }
    }
}