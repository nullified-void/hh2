CREATE DATABASE challenge
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    CONNECTION LIMIT = -1;
CREATE TABLE public.workers
(
    id serial NOT NULL,
    "FirstName" character varying[] NOT NULL,
    "SecondName" character varying[] NOT NULL,
    "LastName" character varying[] NOT NULL,
    "DateOfB" date NOT NULL,
    PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
);

ALTER TABLE public.workers
    OWNER to postgres;
CREATE TABLE public."Logins"
(
    login character varying[] NOT NULL,
    password character varying[] NOT NULL,
    access character varying[] NOT NULL
)
WITH (
    OIDS = FALSE
);

ALTER TABLE public."Logins"
    OWNER to postgres;