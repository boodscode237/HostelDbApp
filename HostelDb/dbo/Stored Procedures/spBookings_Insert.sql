CREATE PROCEDURE [dbo].[spBookings_Insert]
	@roomId int,
	@guestId int,
	@startDate date,
	@endDate date,
	@totalcost money

AS
	begin
		set nocount on;

		insert into dbo.Bookings(RoomId, GuestId, StartDate, EndDate, TotalCost)
		values (@roomId, @guestId, @startDate, @endDate, @totalcost);

	end
RETURN 0