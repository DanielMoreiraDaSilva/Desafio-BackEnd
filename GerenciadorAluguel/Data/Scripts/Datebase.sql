﻿SET CLIENT_ENCODING TO 'UTF8';

DROP TABLE IF EXISTS HIREMOTORCYCLE;
DROP TABLE IF EXISTS USERSYSTEMCNHTYPE;
DROP TABLE IF EXISTS CNHTYPE;
DROP TABLE IF EXISTS USERSYSTEM;
DROP INDEX IF EXISTS index_plate;
DROP TABLE IF EXISTS MOTORCYCLE;

CREATE TABLE MOTORCYCLE (
	ID UUID DEFAULT GEN_RANDOM_UUID(),
	YEAR SMALLINT NOT NULL,
	MODEL VARCHAR(255) NOT NULL,
	PLATE VARCHAR(7) UNIQUE NOT NULL,
	PRIMARY KEY (ID)
);

CREATE INDEX index_plate
ON MOTORCYCLE(PLATE);

CREATE TABLE USERSYSTEM (
	ID UUID DEFAULT GEN_RANDOM_UUID(),
	NAME VARCHAR(255) NOT NULL,
	CNPJ VARCHAR(18) NOT NULL,
	BIRTHDAY DATE NOT NULL,
	CNHNUMBER VARCHAR(12) NOT NULL,
	PRIMARY KEY (ID)
);

CREATE TABLE CNHTYPE (
	ID UUID DEFAULT GEN_RANDOM_UUID(),
	TYPE VARCHAR(3),
	DESCRIPTION VARCHAR(255),
	VALID NUMERIC(1,0) DEFAULT 0,
	PRIMARY KEY (ID)
);

INSERT INTO CNHTYPE(ID, TYPE, DESCRIPTION, VALID) VALUES
('e4b70918-2b9a-4760-bb99-d12cda495f98', 'ACC', 'Bicicleta motorizada / Ciclomotor / Cinquentinha', 0),
('d498282f-ffd9-4649-a4bd-4769ae7b7f07', 'A', 'Moto / Motoneta / Triciclo', 1),
('b835c26e-df70-4b5c-a3b1-07ee06647646', 'B', 'Automóvel / Picape / SUV / Van', 1),
('9d73e963-c2d0-48a4-9eb8-41d5515915c2', 'C', 'Caminhão / Caminhonete / Van de carga', 0),
('59a9b881-f290-4a62-a27f-3688899a121e', 'D', 'Ônibus / Micro-ônibus / Van de passageiros', 0),
('62e00e8e-0ce7-4d89-b967-77eb6b4153dd', 'E', 'Automóvel tracionando trailer / Treminhão / Ônibus articulado', 0);

CREATE TABLE USERSYSTEMCNHTYPE (
	ID UUID DEFAULT GEN_RANDOM_UUID(),
	IDUSERSYSTEM UUID NOT NULL,
	IDCNHTYPE UUID NOT NULL,
	PRIMARY KEY (ID),
	CONSTRAINT FK_USERSYSTEMCNHTYPE FOREIGN KEY(IDUSERSYSTEM) REFERENCES USERSYSTEM(ID),
	CONSTRAINT FK_CNHTYPEUSERSYSTEM FOREIGN KEY(IDCNHTYPE) REFERENCES CNHTYPE(ID)
);

INSERT INTO MOTORCYCLE(ID, YEAR, MODEL, PLATE) VALUES 
('b79e029c-f5b4-4900-9d82-70add9aa1269', 2020, 'Kawasaki', 'FEH8C13'),
('ec1e9b68-2703-4bf3-8b8c-c3be5268ee28', 2021, 'Suzuki', 'BEH8C13');

INSERT INTO USERSYSTEM(ID, NAME, CNPJ, BIRTHDAY, CNHNUMBER) VALUES 
('5f78830e-9044-4b53-8eb8-b4fc93793659', 'Daniel', '00000112312312', '1992-01-24', '123456789123');

CREATE TABLE HIREMOTORCYCLE (
	ID UUID DEFAULT GEN_RANDOM_UUID(),
	IDMOTORCYCLE UUID NOT NULL,
	IDUSERSYSTEM UUID NOT NULL,
	DATESTART DATE NOT NULL,
	DATEEND DATE NULL,
	PRIMARY KEY (ID),
	CONSTRAINT FK_MOTORCYCLE FOREIGN KEY(IDMOTORCYCLE) REFERENCES MOTORCYCLE(ID),
	CONSTRAINT FK_USERSYSTEM FOREIGN KEY(IDUSERSYSTEM) REFERENCES USERSYSTEM(ID)
);

INSERT INTO HIREMOTORCYCLE(ID, IDMOTORCYCLE, IDUSERSYSTEM, DATESTART, DATEEND) VALUES 
('fd2b4d2e-23dc-4d36-8862-45be91b4d063', 'b79e029c-f5b4-4900-9d82-70add9aa1269', '5f78830e-9044-4b53-8eb8-b4fc93793659', '2024-04-04', NULL);

INSERT INTO USERSYSTEM (ID, NAME, CNPJ, BIRTHDAY, CNHNUMBER) VALUES 
(GEN_RANDOM_UUID(), 'João da Silva', '12345678901234', '1990-05-15', '123456789012'),
(GEN_RANDOM_UUID(), 'Maria Oliveira', '98765432109876', '1985-10-20', '987654321098'),
(GEN_RANDOM_UUID(), 'Carlos Santos', '45678901234567', '1978-03-28', '456789012345'),
(GEN_RANDOM_UUID(), 'Ana Souza', '78901234567890', '1995-12-10', '789012345678');