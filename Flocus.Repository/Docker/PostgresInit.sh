psql --username="postgres" --dbname="postgres" -c "CREATE USER flocusUser PASSWORD 'password'"
psql --username="postgres" --dbname="postgres" -l

psql --username "postgres" --dbname "postgres" <<-EOSQL
    CREATE DATABASE flocusdb
           WITH
           OWNER = postgres
           ENCODING = 'UTF8'
           LC_COLLATE = 'en_US.utf8'
		   LC_CTYPE = 'en_US.utf8'
		   TABLESPACE = pg_default
		   CONNECTION LIMIT = -1; 
EOSQL

psql --username "postgres" --dbname "postgres" <<-EOSQL
    GRANT CONNECT ON DATABASE flocusdb TO flocusUser;
	GRANT ALL ON DATABASE flocusdb TO flocusUser;
EOSQL


psql --username="postgres" --dbname="flocusdb" <<-EOSQL

    CREATE TABLE IF NOT EXISTS public.client (
        client_id varchar(20) primary key,
        profile_picture varchar(20),
        account_creation_date date,
        username varchar(20),
        password_hash varchar(100),
        password_salt varchar(100),
        admin_rights boolean
    );

    CREATE TABLE IF NOT EXISTS public.habit (
        habit_id varchar(20) primary key,
        client_id varchar(20) references client(client_id),
        title varchar(20),
        description varchar(400),
        creation_date date,
        streak int,
        highest_streak int
    );

EOSQL

psql --username="postgres" --dbname="flocusdb" <<-EOSQL
    INSERT INTO public.client (client_id, profile_picture, account_creation_date, username, password_hash, password_salt, admin_rights)
    VALUES ('345', 'my profile picture', '1999-01-01', 'lukey', 'passwordHash', 'passwordSalt', true);
EOSQL

psql --username="postgres" --dbname="flocusdb" <<-EOSQL
    INSERT INTO public.habit (habit_id, client_id, title, description, creation_date, streak, highest_streak)
    VALUES ('123', '345', 'Piano', 'gonna play piano every day :)', '1999-01-01', 12, 24);
EOSQL