## Parking Lot Management System

This is a full-stack web application for managing a parking lot, including checking in/out cars, displaying parking lot status, and providing statistics.

**Features:**
- Check-in/Check-out: Operators can check-in/out cars by entering the tag number.
- Real-time Updates: The parking lot status updates dynamically without page reloads.
- Statistics: Provides statistics like available spots, today's revenue, and average daily revenue.
  
**Technologies Used:**
- Backend: C#.Net 12.0, ASP.NET Core 8.0 (razor pages), ADO.Net
- Frontend: Plain JavaScript, Bootstrap 5.1.0
- Database: SQL Server, Stored Procedures, Scaler Functions

**Instructions:**
1. Clone this repository: `git clone https://github.com/hossainmjakaria/cip-plm-project.git`
2. Set up the database using the provided SQL scripts in `PLM.App/PLM.Web/Scripts/DBScript_2316_22032024`.
3. Configure the database connection in the appsettings.json file.
4. Run the .NET Core application.
5. Access the application in your browser.

**Project Structure:**
- PLM.Library
  - `Models/`: Defines the data models used in the application.
  - `Infrustuctures/`: Stores database interaction logic.
  - `Utility/`: Contains utility/helper functions used across the application.
  - `Services/`: Houses the business logic and service layers of the application.

- PLM.Library.Tests
  - `FrontendTests`: Contains Selenium tests for frontend interactions.
  - `ParkingServiceTests`: Contains xUnit tests for testing ParkingService functionality.
  
- PLM.Web
  - `Configurations/`: Contains the dependency injection logic.
  - `Controllers/`: Contains the backend logic for handling HTTP requests.
  - `Models/`: Defines the view models used in the application.
  - `Views/`: Contains the frontend HTML views.
  - `Scripts/`: Includes SQL Database scripts DBScript_2316_22032024.
  - `wwwroot/`: Contains CSS files for styling the frontend and JavaScript files for frontend logic.

**How to Use:**
1. Check-in: Enter the car's tag number and click 'In'.
2. Check-out: Enter the car's tag number and click 'Out'.
3. View Stats: Click 'Stats' to view parking statistics in a modal.
