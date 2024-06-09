psql --username="myuser" --dbname="mydatabase" -l

psql --username "myuser" --dbname "mydatabase" <<-EOSQL
    CREATE DATABASE flocusdb
           WITH
           OWNER = myuser
           ENCODING = 'UTF8'
           LC_COLLATE = 'en_US.utf8'
		   LC_CTYPE = 'en_US.utf8'
		   TABLESPACE = pg_default
		   CONNECTION LIMIT = -1; 
EOSQL

psql --username="myuser" --dbname="flocusdb" <<-EOSQL

    CREATE TABLE IF NOT EXISTS public.client (
        client_id varchar(100) primary key,
        email_address varchar(100),
        account_creation_date timestamptz,
        username varchar(20),
        password_hash varchar(100),
        admin_rights boolean
    );

    CREATE TABLE IF NOT EXISTS public.habit (
        habit_id varchar(100) primary key,
        client_id varchar(100) references public.client(client_id) ON DELETE CASCADE,
        title varchar(20),
        description varchar(400),
        creation_date timestamptz,
        streak int,
        highest_streak int
    );

EOSQL

psql --username="myuser" --dbname="flocusdb" <<-EOSQL
    INSERT INTO public.client (client_id, email_address, account_creation_date, username, password_hash, admin_rights)
    VALUES ('345', 'example@hotmail.co.uk', '1999-01-01', 'lukey', 'passwordHash', true);
EOSQL

psql --username="myuser" --dbname="flocusdb" <<-EOSQL
    INSERT INTO public.habit (habit_id, client_id, title, description, creation_date, streak, highest_streak)
    VALUES ('123', '345', 'Piano', 'gonna play piano every day :)', '1999-01-01', 12, 24);
EOSQL