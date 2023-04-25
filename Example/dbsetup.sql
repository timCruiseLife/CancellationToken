drop table if exists `message_board`;
create table `message_board` (
	`id` bigint not null AUTO_INCREMENT primary key,
	`name` varchar(40) not null,
	`content` varchar(200) null,
	`create_time` bigint not null,
	`update_time` bigint,
	key(`create_time`, `name`)
) engine=InnoDB default charset=utf8mb4;