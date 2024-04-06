SET CLIENT_ENCODING TO 'UTF8';

DROP TABLE IF EXISTS UserNotificationHistory;
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
	Year SMALLINT NOT NULL,  -- Year of manufacture of the motorcycle
	Model VARCHAR(255) NOT NULL,  -- Model of the motorcycle
	Plate CHAR(7) UNIQUE NOT NULL  -- License plate of the motorcycle
);

CREATE INDEX index_plate
ON Motorcycle(Plate);

INSERT INTO Motorcycle(ID, Year, Model, Plate) VALUES 
('b79e029c-f5b4-4900-9d82-70add9aa1269', 2020, 'Kawasaki', 'FEH8C13'),
('ec1e9b68-2703-4bf3-8b8c-c3be5268ee28', 2021, 'Suzuki', 'BEH8C13');

CREATE TABLE UserSystem (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	Name VARCHAR(255) NOT NULL,  -- Name of the user
	Cnpj CHAR(18) NOT NULL,  -- CNPJ of the user
	Birthday DATE NOT NULL,  -- Birthdate of the user
	CnhNumber VARCHAR(12) NOT NULL,  -- Driver's license number of the user
	CnhImagePath VARCHAR(255) NULL  -- Path to the user's driver's license image
);

INSERT INTO UserSystem(ID, Name, Cnpj, Birthday, CnhNumber, CnhImagePath) VALUES 
('5f78830e-9044-4b53-8eb8-b4fc93793659', 'Daniel', '00000112312312', '1992-01-24', '123456789123', 'd38de5fc-1ebf-4482-a514-f689363eef1f.png'),
('6d0a08a7-6187-4300-87a0-db839a59366d', 'João da Silva', '12345678901234', '1990-05-15', '123456789012', NULL),
('d92ba1bc-8850-4c47-a36d-d0a98f90d900', 'Maria Oliveira', '98765432109876', '1985-10-20', '987654321098', NULL),
('b328bb2c-e5e2-43a6-9c52-63c9e8d25d12', 'Carlos Santos', '45678901234567', '1978-03-28', '456789012345', NULL),
('4804b88e-8b09-48cc-b3b4-d36e49957971', 'Ana Souza', '78901234567890', '1995-12-10', '789012345678', NULL);

CREATE TABLE CnhType (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	Type VARCHAR(3),  -- Type of driver's license (A, B, C, etc.)
	Description VARCHAR(255),  -- Description of the driver's license type
	Valid NUMERIC(1,0) DEFAULT 0  -- Indicates if the driver's license type is valid
);

INSERT INTO CnhType(ID, Type, Description, Valid) VALUES
('e4b70918-2b9a-4760-bb99-d12cda495f98', 'ACC', 'Bicicleta motorizada / Ciclomotor / Cinquentinha', 0),
('d498282f-ffd9-4649-a4bd-4769ae7b7f07', 'A', 'Moto / Motoneta / Triciclo', 1),
('b835c26e-df70-4b5c-a3b1-07ee06647646', 'B', 'Automóvel / Picape / SUV / Van', 1),
('9d73e963-c2d0-48a4-9eb8-41d5515915c2', 'C', 'Caminhão / Caminhonete / Van de carga', 0),
('59a9b881-f290-4a62-a27f-3688899a121e', 'D', 'Ônibus / Micro-ônibus / Van de passageiros', 0),
('62e00e8e-0ce7-4d89-b967-77eb6b4153dd', 'E', 'Automóvel tracionando trailer / Treminhão / Ônibus articulado', 0);

CREATE TABLE UserSystemCnhType (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	IdUserSystem UUID NOT NULL,  -- ID of the associated user
	IdCnhType UUID NOT NULL,  -- ID of the associated driver's license type
	CONSTRAINT FK_UserSystem_UserSystemCnhType FOREIGN KEY(IdUserSystem) REFERENCES UserSystem(ID),
	CONSTRAINT FK_CnhType_CnhTypeUserSystem FOREIGN KEY(IdCnhType) REFERENCES CnhType(ID)
);

-- Daniel possui CNH dos tipos A e B
INSERT INTO UserSystemCnhType (ID, IdUserSystem, IdCnhType) VALUES
('8a19cbbf-1633-4e9a-9a76-1f15b9b32843', '5f78830e-9044-4b53-8eb8-b4fc93793659', 'd498282f-ffd9-4649-a4bd-4769ae7b7f07'), -- CNH tipo A
('be3d2e43-3c4b-4f61-9a90-5fca3cb44621', '5f78830e-9044-4b53-8eb8-b4fc93793659', 'b835c26e-df70-4b5c-a3b1-07ee06647646'); -- CNH tipo B

-- João da Silva possui CNH do tipo B
INSERT INTO UserSystemCnhType (ID, IdUserSystem, IdCnhType) VALUES
('80ec91a6-f85d-41ec-a6d2-45f8945f6789', '6d0a08a7-6187-4300-87a0-db839a59366d', 'b835c26e-df70-4b5c-a3b1-07ee06647646'); -- CNH tipo B

CREATE TABLE RentalPlan (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    Name VARCHAR(50) NOT NULL,  -- Name of the rental plan
    DurationInDays INT NOT NULL,  -- Duration of the plan in days
    DailyCost DECIMAL(10, 2) NOT NULL,  -- Daily cost of the plan
    FactorCalculateCost DECIMAL(10, 2) NOT NULL,  -- Additional cost calculation factor
	CostExtraDays DECIMAL(10, 2) NOT NULL  -- Additional cost for extra days
);

INSERT INTO RentalPlan (ID, Name, DurationInDays, DailyCost, FactorCalculateCost, CostExtraDays) VALUES 
('b2819ee8-6809-4527-853d-6c5f9bdf573e', 'Plano 7 dias', 7, 30.00, 0.20, 50.00),
('529b9f47-9f51-4ad3-bf25-b8363f1889c7', 'Plano 15 dias', 15, 28.00, 0.40, 50.00),
('ee64dca2-b39c-4ee8-9cad-3e25e53c1dd4', 'Plano 30 dias', 30, 22.00, 0.60, 50.00);

CREATE TABLE MotorcycleRental (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    IdUser UUID NOT NULL,  -- ID of the associated user for the rental
    IdMotorcycle UUID NOT NULL,  -- ID of the associated motorcycle for the rental
    IdRentalPlan UUID NOT NULL,  -- ID of the associated rental plan
    RentalStartDate DATE NOT NULL,  -- Start date of the rental
    RentalEndDate DATE NOT NULL,  -- End date of the rental
	ExpectedReturnDate DATE NOT NULL,  -- Expected return date of the rental
    CONSTRAINT FK_User_MotorcycleRental FOREIGN KEY (IdUser) REFERENCES UserSystem(ID),
    CONSTRAINT FK_Motorcycle_MotorcycleRental FOREIGN KEY (IdMotorcycle) REFERENCES Motorcycle(ID),
    CONSTRAINT FK_RentalPlan_MotorcycleRental FOREIGN KEY (IdRentalPlan) REFERENCES RentalPlan(ID)
);

INSERT INTO MotorcycleRental (ID, IdUser, IdMotorcycle, IdRentalPlan, RentalStartDate, RentalEndDate, ExpectedReturnDate)
VALUES ('61cdf275-af9a-4655-ba6f-8f13d2fe47d5', '5f78830e-9044-4b53-8eb8-b4fc93793659', 'b79e029c-f5b4-4900-9d82-70add9aa1269', 'b2819ee8-6809-4527-853d-6c5f9bdf573e', '2024-04-05', '2024-04-12', '2024-04-10');

CREATE TABLE StatusDeliveryOrder (
	ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	Status VARCHAR(255)  -- Status of the delivery order (Disponível, Aceito, Entregue, etc.)
);

INSERT INTO StatusDeliveryOrder (ID, Status) VALUES 
('8c97d6f1-0d2d-4f5a-aa0d-7f05f9d59d0d', 'Disponível'),
('b91d2876-0df4-4373-92b6-9e1bde27d66b', 'Aceito'),
('eadc1d7d-ffdb-45c0-aa12-c03e4a4e86b8', 'Entregue');

CREATE TABLE DeliveryOrder (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
	DataCreate DATE NOT NULL,  -- Creation date of the delivery order
	CostDelivery DECIMAL(10, 2) NOT NULL,  -- Delivery cost
    IdStatusDeliveryOrder UUID NOT NULL,  -- ID of the delivery order status
	CONSTRAINT FK_StatusDeliveryOrder_DeliveryOrder FOREIGN KEY (IdStatusDeliveryOrder) REFERENCES StatusDeliveryOrder(ID)
);

INSERT INTO DeliveryOrder (ID, DataCreate, CostDelivery, IdStatusDeliveryOrder) 
VALUES ('8c97d6f1-0d2d-4f5a-aa0d-7f05f9d59d0d', '2024-04-05', 50.00, '8c97d6f1-0d2d-4f5a-aa0d-7f05f9d59d0d');

CREATE TABLE UserNotificationHistory (
    ID UUID PRIMARY KEY DEFAULT GEN_RANDOM_UUID(),
    IdUser UUID NOT NULL, -- ID of the user associated with the notification history
    IdDeliveryOrder UUID NOT NULL, -- ID of the delivery order associated with the notification history
    CONSTRAINT FK_User_UserNotificationHistory FOREIGN KEY (IdUser) REFERENCES UserSystem(ID),
    CONSTRAINT FK_DeliveryOrder_UserNotificationHistory FOREIGN KEY (IdDeliveryOrder) REFERENCES DeliveryOrder(ID)
);

INSERT INTO UserNotificationHistory (ID, IdUser, IdDeliveryOrder) 
VALUES ('8c97d6f1-0d2d-4f5a-aa0d-7f05f9d59d0d', '5f78830e-9044-4b53-8eb8-b4fc93793659', '8c97d6f1-0d2d-4f5a-aa0d-7f05f9d59d0d');