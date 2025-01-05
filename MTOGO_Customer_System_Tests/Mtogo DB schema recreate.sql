CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    OrderDate DATETIME,
    OrderStatus NVARCHAR(50),
    Total DECIMAL(10, 2) NOT NULL,
);

CREATE TABLE MenuItems  (
	MenuItemID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NOT NULL,	
    Price DECIMAL(10, 2) NOT NULL,
	Category NVARCHAR(50),
);

CREATE TABLE OrderLines (
	OrderLineID INT PRIMARY KEY IDENTITY(1,1),
	Quantity INT NOT NULL,
);


CREATE TABLE Couriers(
	CourierID INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(100) NOT NULL,
	Phone NVARCHAR(10) NOT NULL,
	City NVARCHAR(100) NOT NULL,
);

CREATE TABLE CourierOrders(
	CourierOrderID INT PRIMARY KEY IDENTITY(1,1),
);


CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Phone NVARCHAR(15) NOT NULL,
    Address NVARCHAR(255)NOT NULL,
    City NVARCHAR(100)NOT NULL,
	Password NVARCHAR(85) NOT NULL
);


CREATE TABLE Restaurants (
	RestaurantID INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Phone NVARCHAR(15) NOT NULL,
    Address NVARCHAR(255)NOT NULL,
    City NVARCHAR(100)NOT NULL,
	Password NVARCHAR(85) NOT NULL
);

CREATE TABLE CustomerReviews(
	CustomerReviewID INT PRIMARY KEY IDENTITY(1,1),
	Text NVARCHAR(255),
	Rating INT
);

CREATE TABLE RestaurantComplaints(
	RestaurantComplaintID INT PRIMARY KEY IDENTITY(1,1),
	Text NVARCHAR(255) NOT NULL
);


ALTER TABLE Orders
ADD FK_Orders_RestaurantID INT, FK_Orders_CustomerID INT,
	CONSTRAINT FK_Orders_RestaurantID FOREIGN KEY (FK_Orders_RestaurantID) REFERENCES Restaurants(RestaurantID),
	CONSTRAINT FK_Orders_CustomerID FOREIGN KEY (FK_Orders_CustomerID) REFERENCES Customers(CustomerID);

ALTER TABLE MenuItems
ADD FK_MenuItems_RestaurantID INT,
	CONSTRAINT FK_MenuItems_RestaurantID FOREIGN KEY (FK_MenuItems_RestaurantID) REFERENCES Restaurants(RestaurantID);

ALTER TABLE OrderLines
ADD FK_OrderLines_OrderID INT,FK_OrderLines_MenuItemID INT, FK_OrderLines_RestaurantID INT,
	CONSTRAINT FK_OrderLines_OrderID FOREIGN KEY (FK_OrderLines_OrderID) REFERENCES Orders (OrderID),
	CONSTRAINT FK_OrderLines_MenuItemID FOREIGN KEY (FK_OrderLines_MenuItemID) REFERENCES MenuItems (MenuItemID),
	CONSTRAINT FK_OrderLines_RestaurantID FOREIGN KEY (FK_OrderLines_RestaurantID) REFERENCES Restaurants (RestaurantID);

ALTER TABLE CourierOrders
ADD FK_CourierOrders_OrderID INT, FK_CourierOrders_CourierID INT,
CONSTRAINT FK_CourierOrders_OrderID FOREIGN KEY (FK_CourierOrders_OrderID) REFERENCES Orders(OrderID),
CONSTRAINT FK_CourierOrders_CourierID FOREIGN KEY (FK_CourierOrders_CourierID) REFERENCES Couriers(CourierID);

ALTER TABLE CustomerReviews
ADD FK_CustomerReviews_OrderID INT
CONSTRAINT FK_CustomerReviews_OrderID FOREIGN KEY (FK_CustomerReviews_OrderID) REFERENCES Orders(OrderID);

ALTER TABLE RestaurantComplaints
ADD FK_RestaurantComplaints_OrderID INT
CONSTRAINT FK_RestaurantComplaints_OrderID FOREIGN KEY (FK_RestaurantComplaints_OrderID) REFERENCES Orders(OrderID);