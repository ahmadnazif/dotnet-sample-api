# Sample API
A very simple API with typical REST endpoints, with a SignalR service

## Database
- Uses MySQL database
- Create `phonebook` database:

```sql
CREATE DATABASE IF NOT EXISTS `phonebook`
USE `phonebook`;

CREATE TABLE IF NOT EXISTS `contact` (
  `id` int NOT NULL AUTO_INCREMENT,
  `firstname` varchar(50) DEFAULT NULL,
  `lastname` varchar(50) DEFAULT NULL,
  `num` varchar(50) DEFAULT NULL,
  `created_time` timestamp NOT NULL DEFAULT (now()),
  `update_time` timestamp NOT NULL DEFAULT (now()) ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
```
-  Create & assign user for this database with full priviledges
