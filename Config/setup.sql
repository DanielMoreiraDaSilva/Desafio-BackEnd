SET CLIENT_ENCODING TO 'UTF8';

DROP TABLE IF EXISTS DeliveryOrderNotifyHistory;
DROP TABLE IF EXISTS LogApp;
DROP TABLE IF EXISTS DeliveryOrderAcceptance;
DROP TABLE IF EXISTS UserNotifyHistory;
DROP TABLE IF EXISTS DeliveryOrder;
DROP TABLE IF EXISTS StatusDeliveryOrder;
DROP TABLE IF EXISTS MotorcycleRental;
DROP TABLE IF EXISTS RentalPlan;
DROP TABLE IF EXISTS UserSystemCnhType;
DROP TABLE IF EXISTS CnhType;
DROP TABLE IF EXISTS UserSystem;
DROP INDEX IF EXISTS index_plate;
DROP TABLE IF EXISTS Motorcycle;

CREATE TABLE Motorcycle (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	Year SMALLINT NOT NULL,
	Model VARCHAR(255) NOT NULL,
	Plate CHAR(7) UNIQUE NOT NULL
);

COMMENT ON COLUMN Motorcycle.Year IS 'Year of manufacture of the motorcycle';
COMMENT ON COLUMN Motorcycle.Model IS 'Model of the motorcycle';
COMMENT ON COLUMN Motorcycle.Plate IS 'License plate of the motorcycle';

CREATE INDEX index_plate
ON Motorcycle(Plate);

INSERT INTO Motorcycle(ID, Year, Model, Plate) VALUES 
('b79e029c-f5b4-4900-9d82-70add9aa1269', 2020, 'Kawasaki', 'FEH8C13'),
('ec1e9b68-2703-4bf3-8b8c-c3be5268ee28', 2021, 'Suzuki', 'BEH8C13');

CREATE TABLE UserSystem (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	Name VARCHAR(255) NOT NULL, 
	Cnpj CHAR(18) NOT NULL,
	Birthday DATE NOT NULL,
	CnhNumber VARCHAR(12) NOT NULL,
	CnhImagePath VARCHAR(255) NULL
);

COMMENT ON COLUMN UserSystem.Name IS 'Name of the user';
COMMENT ON COLUMN UserSystem.Cnpj IS 'CNPJ of the user';
COMMENT ON COLUMN UserSystem.Birthday IS 'Birthdate of the user';
COMMENT ON COLUMN UserSystem.CnhNumber IS 'Driver´s license number of the user';
COMMENT ON COLUMN UserSystem.CnhImagePath IS 'Path to the user´s driver´s license image';

INSERT INTO UserSystem(ID, Name, Cnpj, Birthday, CnhNumber, CnhImagePath) VALUES 
('5f78830e-9044-4b53-8eb8-b4fc93793659', 'Daniel', '00000112312312', '1992-01-24', '123456789123', 'd38de5fc-1ebf-4482-a514-f689363eef1f.png'),
('6d0a08a7-6187-4300-87a0-db839a59366d', 'João da Silva', '12345678901234', '1990-05-15', '123456789012', NULL),
('d92ba1bc-8850-4c47-a36d-d0a98f90d900', 'Maria Oliveira', '98765432109876', '1985-10-20', '987654321098', NULL),
('b328bb2c-e5e2-43a6-9c52-63c9e8d25d12', 'Carlos Santos', '45678901234567', '1978-03-28', '456789012345', NULL),
('4804b88e-8b09-48cc-b3b4-d36e49957971', 'Ana Souza', '78901234567890', '1995-12-10', '789012345678', NULL);

CREATE TABLE CnhType (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	Type VARCHAR(3),
	Description VARCHAR(255),
	Valid NUMERIC(1,0) DEFAULT 0
);

COMMENT ON COLUMN CnhType.Type IS 'Type of driver´s license (A, B, C, etc.)';
COMMENT ON COLUMN CnhType.Description IS 'Description of the driver´s license type';
COMMENT ON COLUMN CnhType.Valid IS 'Indicates if the driver´s license type is valid';

INSERT INTO CnhType(ID, Type, Description, ValID) VALUES
('e4b70918-2b9a-4760-bb99-d12cda495f98', 'ACC', 'Bicicleta motorizada / Ciclomotor / Cinquentinha', 0),
('d498282f-ffd9-4649-a4bd-4769ae7b7f07', 'A', 'Moto / Motoneta / Triciclo', 1),
('b835c26e-df70-4b5c-a3b1-07ee06647646', 'B', 'Automóvel / Picape / SUV / Van', 1),
('9d73e963-c2d0-48a4-9eb8-41d5515915c2', 'C', 'Caminhão / Caminhonete / Van de carga', 0),
('59a9b881-f290-4a62-a27f-3688899a121e', 'D', 'Ônibus / Micro-ônibus / Van de passageiros', 0),
('62e00e8e-0ce7-4d89-b967-77eb6b4153dd', 'E', 'Automóvel tracionando trailer / Treminhão / Ônibus articulado', 0);

CREATE TABLE UserSystemCnhType (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	IDUserSystem UUID NOT NULL,
	IDCnhType UUID NOT NULL,
	CONSTRAINT FK_UserSystem_UserSystemCnhType FOREIGN KEY(IDUserSystem) REFERENCES UserSystem(ID),
	CONSTRAINT FK_CnhType_CnhTypeUserSystem FOREIGN KEY(IDCnhType) REFERENCES CnhType(ID)
);

COMMENT ON COLUMN UserSystemCnhType.IDUserSystem IS 'ID of the associated user';
COMMENT ON COLUMN UserSystemCnhType.IDCnhType IS 'ID of the associated driver´s license type';

-- Daniel possui CNH dos tipos A e B
INSERT INTO UserSystemCnhType (ID, IDUserSystem, IDCnhType) VALUES
('8a19cbbf-1633-4e9a-9a76-1f15b9b32843', '5f78830e-9044-4b53-8eb8-b4fc93793659', 'd498282f-ffd9-4649-a4bd-4769ae7b7f07'), -- CNH tipo A
('be3d2e43-3c4b-4f61-9a90-5fca3cb44621', '5f78830e-9044-4b53-8eb8-b4fc93793659', 'b835c26e-df70-4b5c-a3b1-07ee06647646'); -- CNH tipo B

-- João da Silva possui CNH do tipo B
INSERT INTO UserSystemCnhType (ID, IDUserSystem, IDCnhType) VALUES
('80ec91a6-f85d-41ec-a6d2-45f8945f6789', '6d0a08a7-6187-4300-87a0-db839a59366d', 'b835c26e-df70-4b5c-a3b1-07ee06647646'); -- CNH tipo B

CREATE TABLE RentalPlan (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    Name VARCHAR(50) NOT NULL,
    DurationInDays INT NOT NULL,
    DailyCost DECIMAL(10, 2) NOT NULL,
    FactorCalculateCost DECIMAL(10, 2) NOT NULL,
	CostExtraDays DECIMAL(10, 2) NOT NULL
);

COMMENT ON COLUMN RentalPlan.Name IS 'Name of the rental plan';
COMMENT ON COLUMN RentalPlan.DurationInDays IS 'Duration of the plan in days';
COMMENT ON COLUMN RentalPlan.DailyCost IS 'Daily cost of the plan';
COMMENT ON COLUMN RentalPlan.FactorCalculateCost IS 'Additional cost calculation factor';
COMMENT ON COLUMN RentalPlan.CostExtraDays IS 'Additional cost for extra days';

INSERT INTO RentalPlan (ID, Name, DurationInDays, DailyCost, FactorCalculateCost, CostExtraDays) VALUES 
('b2819ee8-6809-4527-853d-6c5f9bdf573e', 'Plano 7 dias', 7, 30.00, 0.20, 50.00),
('529b9f47-9f51-4ad3-bf25-b8363f1889c7', 'Plano 15 dias', 15, 28.00, 0.40, 50.00),
('ee64dca2-b39c-4ee8-9cad-3e25e53c1dd4', 'Plano 30 dias', 30, 22.00, 0.60, 50.00);

CREATE TABLE MotorcycleRental (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    IDUser UUID NOT NULL,
    IDMotorcycle UUID NOT NULL,
    IDRentalPlan UUID NOT NULL,
    RentalStartDate DATE NOT NULL,
    RentalEndDate DATE NOT NULL,
	ExpectedReturnDate DATE NOT NULL,
    CONSTRAINT FK_User_MotorcycleRental FOREIGN KEY (IDUser) REFERENCES UserSystem(ID),
    CONSTRAINT FK_Motorcycle_MotorcycleRental FOREIGN KEY (IDMotorcycle) REFERENCES Motorcycle(ID),
    CONSTRAINT FK_RentalPlan_MotorcycleRental FOREIGN KEY (IDRentalPlan) REFERENCES RentalPlan(ID)
);

COMMENT ON COLUMN MotorcycleRental.IDUser IS 'ID of the associated user for the rental';
COMMENT ON COLUMN MotorcycleRental.IDMotorcycle IS 'ID of the associated motorcycle for the rental';
COMMENT ON COLUMN MotorcycleRental.IDRentalPlan IS 'ID of the associated rental plan';
COMMENT ON COLUMN MotorcycleRental.RentalStartDate IS 'Start date of the rental';
COMMENT ON COLUMN MotorcycleRental.RentalEndDate IS 'End date of the rental';
COMMENT ON COLUMN MotorcycleRental.ExpectedReturnDate IS 'Expected return date of the rental';

INSERT INTO MotorcycleRental (ID, IDUser, IDMotorcycle, IDRentalPlan, RentalStartDate, RentalEndDate, ExpectedReturnDate)
VALUES ('61cdf275-af9a-4655-ba6f-8f13d2fe47d5', '5f78830e-9044-4b53-8eb8-b4fc93793659', 'b79e029c-f5b4-4900-9d82-70add9aa1269', 'b2819ee8-6809-4527-853d-6c5f9bdf573e', '2024-04-05', '2024-04-12', '2024-04-10');

CREATE TABLE StatusDeliveryOrder (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	Status VARCHAR(255)
);

COMMENT ON COLUMN StatusDeliveryOrder.Status IS 'Status of the delivery order (Disponível, Aceito, Entregue, etc.)';

INSERT INTO StatusDeliveryOrder (ID, Status) VALUES 
('542b4f8b-0a99-4f1b-822e-f6378302d800', 'Disponível'),
('b91d2876-0df4-4373-92b6-9e1bde27d66b', 'Aceito'),
('eadc1d7d-ffdb-45c0-aa12-c03e4a4e86b8', 'Entregue');

CREATE TABLE DeliveryOrder (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	DataCreate TIMESTAMP NOT NULL,
	CostDelivery DECIMAL(10, 2) NOT NULL,
    IDStatusDeliveryOrder UUID NOT NULL,
	CONSTRAINT FK_StatusDeliveryOrder_DeliveryOrder FOREIGN KEY (IDStatusDeliveryOrder) REFERENCES StatusDeliveryOrder(ID)
);

COMMENT ON COLUMN DeliveryOrder.DataCreate IS 'Creation date of the delivery order';
COMMENT ON COLUMN DeliveryOrder.CostDelivery IS 'Delivery cost';
COMMENT ON COLUMN DeliveryOrder.IDStatusDeliveryOrder IS 'ID of the delivery order status';

INSERT INTO DeliveryOrder (ID, DataCreate, CostDelivery, IDStatusDeliveryOrder) VALUES
('8c97d6f1-0d2d-4f5a-aa0d-7f05f9d59d0d', '2024-04-05', 50.00, '542b4f8b-0a99-4f1b-822e-f6378302d800');

CREATE TABLE DeliveryOrderAcceptance (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    IDUser UUID NOT NULL,
    IDDeliveryOrder UUID NOT NULL,
    AcceptanceDateTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_User_DeliveryOrderAcceptance FOREIGN KEY (IDUser) REFERENCES UserSystem(ID),
    CONSTRAINT FK_DeliveryOrder_DeliveryOrderAcceptance FOREIGN KEY (IDDeliveryOrder) REFERENCES DeliveryOrder(ID)
);

COMMENT ON COLUMN DeliveryOrderAcceptance.IDUser IS 'ID of the user (delivery person) who accepts the delivery order';
COMMENT ON COLUMN DeliveryOrderAcceptance.IDDeliveryOrder IS 'ID of the delivery order being accepted';
COMMENT ON COLUMN DeliveryOrderAcceptance.AcceptanceDateTime IS 'Date and time when the delivery order is accepted';

CREATE TABLE LogApp(
  ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
  DateTimeLog TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  Level VARCHAR(255) NOT NULL,
  Message VARCHAR(4000) NOT NULL,
  Origin VARCHAR(255),
  Address VARCHAR(255),
  Exception VARCHAR(4000)
);

COMMENT ON COLUMN LogApp.DateTimeLog IS 'Date and time when the log entry was recorded';
COMMENT ON COLUMN LogApp.Level IS 'The severity level of the log message';
COMMENT ON COLUMN LogApp.Message IS 'The content of the log message';
COMMENT ON COLUMN LogApp.Origin IS 'The source or origin of the log message';
COMMENT ON COLUMN LogApp.Address IS 'The IP address associated with the log message';
COMMENT ON COLUMN LogApp.Exception IS 'Details of any exception associated with the log message';

CREATE TABLE DeliveryOrderNotifyHistory (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    IDDeliveryOrder UUID NOT NULL,
    DataDeliveryOrderCreate TIMESTAMP NOT NULL,
    DateNotify TIMESTAMP NOT NULL,
    CONSTRAINT FK_DeliveryOrderNotifyHistory_DeliveryOrder FOREIGN KEY (IDDeliveryOrder) REFERENCES DeliveryOrder(ID)
);

COMMENT ON COLUMN DeliveryOrderNotifyHistory.IDDeliveryOrder IS 'The identifier of the associated delivery order';
COMMENT ON COLUMN DeliveryOrderNotifyHistory.DataDeliveryOrderCreate IS 'The date when the delivery order was created';
COMMENT ON COLUMN DeliveryOrderNotifyHistory.DateNotify IS 'The identifier of the notification date';

CREATE TABLE UserNotifyHistory (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    IDUser UUID NOT NULL,
    IDDeliveryOrder UUID NOT NULL,
    CONSTRAINT FK_User_UserNotifyHistory FOREIGN KEY (IDUser) REFERENCES UserSystem(ID),
    CONSTRAINT FK_DeliveryOrder_UserNotifyHistory FOREIGN KEY (IDDeliveryOrder) REFERENCES DeliveryOrder(ID)
);

COMMENT ON COLUMN UserNotifyHistory.IDUser IS 'ID of the user associated with the notification history';
COMMENT ON COLUMN UserNotifyHistory.IDDeliveryOrder IS 'ID of the delivery order associated with the notification history';

INSERT INTO UserNotifyHistory (ID, IDUser, IDDeliveryOrder) VALUES 
('9624859e-0ecd-40d8-a26a-fd0b60752b17', '5f78830e-9044-4b53-8eb8-b4fc93793659', '8c97d6f1-0d2d-4f5a-aa0d-7f05f9d59d0d'),
('c72e6fa7-d669-4ff6-9e5f-c9571985c628', '6d0a08a7-6187-4300-87a0-db839a59366d', '8c97d6f1-0d2d-4f5a-aa0d-7f05f9d59d0d');