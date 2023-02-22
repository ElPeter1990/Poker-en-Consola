

create table Player(id bigserial not null primary key, username varchar (20) not null, nickname varchar (20) not null, password varchar (20) not null, email varchar (50) not null, realMoney DECIMAL(10, 2) NOT NULL DEFAULT 0, bonus DECIMAL(10, 2) NOT NULL DEFAULT 0);

insert into player(username, nickname, "password", email) values ('Peter', 'Peter', 'Peter' ,'petercastro1990@gmail.com');  
