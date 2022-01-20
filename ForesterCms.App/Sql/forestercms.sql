CREATE DATABASE  IF NOT EXISTS `forestercms` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `forestercms`;
-- MySQL dump 10.13  Distrib 8.0.25, for Win64 (x86_64)
--
-- Host: localhost    Database: forestercms
-- ------------------------------------------------------
-- Server version	8.0.25

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cms_branch`
--

DROP TABLE IF EXISTS `cms_branch`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cms_branch` (
  `id` int NOT NULL,
  `lcid` int NOT NULL,
  `alias` varchar(50) DEFAULT NULL,
  `system` bit(1) NOT NULL,
  `parentid` int DEFAULT NULL,
  PRIMARY KEY (`id`,`lcid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_branch`
--

LOCK TABLES `cms_branch` WRITE;
/*!40000 ALTER TABLE `cms_branch` DISABLE KEYS */;
INSERT INTO `cms_branch` VALUES (1,1037,'entities',_binary '',8),(2,1037,'groups',_binary '',7),(3,1037,'users',_binary '',7),(4,1037,'language-resources',_binary '',NULL),(5,1037,'data',_binary '',NULL),(6,1037,'site1',_binary '\0',NULL),(7,1037,'admin',_binary '',NULL),(8,1037,'developer',_binary '',NULL);
/*!40000 ALTER TABLE `cms_branch` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_entity_info`
--

DROP TABLE IF EXISTS `cms_entity_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cms_entity_info` (
  `id` int NOT NULL,
  `name` varchar(100) DEFAULT NULL,
  `alias` varchar(50) DEFAULT NULL,
  `multilang` bit(1) NOT NULL,
  `properties` longtext,
  `system` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `alias_UNIQUE` (`alias`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_entity_info`
--

LOCK TABLES `cms_entity_info` WRITE;
/*!40000 ALTER TABLE `cms_entity_info` DISABLE KEYS */;
INSERT INTO `cms_entity_info` VALUES (1,'ישויות','entity_info',_binary '\0',NULL,_binary ''),(2,'ענפים','branch',_binary '',NULL,_binary ''),(3,'רכיב דף','component',_binary '',NULL,_binary '');
/*!40000 ALTER TABLE `cms_entity_info` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_language`
--

DROP TABLE IF EXISTS `cms_language`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cms_language` (
  `id` int NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  `alias` varchar(45) DEFAULT NULL,
  `isRtl` tinyint NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_language`
--

LOCK TABLES `cms_language` WRITE;
/*!40000 ALTER TABLE `cms_language` DISABLE KEYS */;
INSERT INTO `cms_language` VALUES (1,'עברית','heb',1),(2,'English','eng',0);
/*!40000 ALTER TABLE `cms_language` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_object`
--

DROP TABLE IF EXISTS `cms_object`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cms_object` (
  `objid` int NOT NULL,
  `eid` int NOT NULL,
  `lcid` int NOT NULL DEFAULT '0',
  `bid` int NOT NULL,
  `name` varchar(50) DEFAULT NULL,
  `status` tinyint(1) DEFAULT '2',
  `createdate` datetime DEFAULT CURRENT_TIMESTAMP,
  `updatedate` datetime DEFAULT CURRENT_TIMESTAMP,
  `sort` int DEFAULT NULL,
  PRIMARY KEY (`objid`,`eid`,`lcid`,`bid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_object`
--

LOCK TABLES `cms_object` WRITE;
/*!40000 ALTER TABLE `cms_object` DISABLE KEYS */;
INSERT INTO `cms_object` VALUES (1,2,1037,0,'ישויות',1,'2021-07-25 00:54:37','2021-07-25 00:54:37',6),(2,2,1037,0,'קבוצות',1,'2021-07-25 00:54:37','2021-07-25 00:54:37',5),(3,2,1037,0,'משתמשים',1,'2021-07-25 00:54:37','2021-07-25 00:54:37',4),(4,2,1037,0,'משאבי שפה',1,'2021-07-25 00:54:37','2021-07-25 00:54:37',3),(5,2,1037,0,'מידע',1,'2021-07-25 00:54:37','2021-07-25 00:54:37',2),(6,2,1037,0,'אתר 1',1,'2021-07-25 00:54:37','2021-07-25 00:54:37',1),(7,2,1037,0,'ניהול',1,'2021-07-25 00:54:37','2021-07-25 00:54:37',7),(8,2,1037,0,'פיתוח',1,'2021-07-25 00:54:37','2021-07-25 00:54:37',8);
/*!40000 ALTER TABLE `cms_object` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cms_xml_repository`
--

DROP TABLE IF EXISTS `cms_xml_repository`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cms_xml_repository` (
  `key` varchar(50) NOT NULL,
  `xml` varchar(4000) DEFAULT NULL,
  `createdate` datetime NOT NULL,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cms_xml_repository`
--

LOCK TABLES `cms_xml_repository` WRITE;
/*!40000 ALTER TABLE `cms_xml_repository` DISABLE KEYS */;
INSERT INTO `cms_xml_repository` VALUES ('key-62767e89-16b8-4944-9517-cbbaa89d8016','j5aM9Li2bFA4NoUMWhdwBzqw8K38j0+L8YiB7Nw781sVgJa8bsmrMTGOZsWETDht30swXjb9z/umLOnGvzVB9RXXUvuOAbSpdpLWiIHcB9gOXe5sDpS07xmCsE2FRhcI2gZzseA0SI4jE19+JXrxh5bRduWsQwzEyJ+cQNtRpD6g5uoPNP3vquNNx52bWkkBS4JPmLfU5eZvlTxbw/vFPpPeEMlTNMA+KXwYQqXhuqwCG2T+KTF4cYKEM1df294Fo3RjWRn/79DVFl71OEY5ZkibaVXlW/OvE/mjJx5QhL136lPELJIBgEYNxqqC2/Ik7/mH361WUlf2yZ2kJVLQELcqVVjFhY0k9/B8Qfs0MduhLBY2cyMBUIVy2LG5Z6y4rr+8ljGxgLPlWCFGaE322eIadlIwVLxy0V2Pcg3o65SMSDEp0uH0x8hIcxR9y/wK60+R+MU9aVchMz2vsGwpTuXd3H59d8vVbihgsncuaI+rEnzFRZOO/0QfWs4A8Xvx+mj2SG8pQqXpvgYSGwf6zlblCDRKXu70zbLawIPiXWnsvCiZbHcoPWDBy6bEN2sDnWoLwP5gUbCgTI1jpzDuZB7Fs+LRu8EvJdG13D1qDKX+x2A4WiCl7vSQPYuRuozXMuA/CEtg4sJxkfD5KcSXwP7oItJMoRord66N0cSf1Xk0fTMbE5j/F8OKZCfEjFboJvMgR0LApJL9F/hSlUlScYW/MnUbO2psGRlsKLi7oTfgXED9L4+7ysAKh+Nf7aJBy3XhiWaKr6vfVP16/3Gv6yuvL6mW68yLuPL2ecL5QlQRbTXUavCA0iTnxN5aB0fMB4rj6jnErE/6JIX21sf2DUJW4KkJKN54HhQrrT9+fQt7EKy2IoU7E9TtcZ/0mjKpHM+FGdSAjahQazXyjbxVaYpt/lT7M9mGegPNmjKqHqqacffVJDCxBAJZ+cTk91qRQeCEIIxa2jriJfonVFdA5hzIs7PdUoEgS4r0vxy/+/chteZz1Gpisay95n/X30FsIxJjdZnheTjpDJ1ZneXfNlGxxTgeAOq+rWhPvWa20sU8mGPt/vvZ5w9AKsZm7GnCje7qnDdYoU1rkfSka2//Dq1uPRzjPmpaDw5QqhT+XLICnc8h2HhxTyndf7/N6ZpjPsI2StYdbaHS2+CCgg+NS1vwHIRmjJpCQGzs0Hm9GA8=.5BeA37jDby+n3lIORngI7A==','2021-07-30 00:00:00'),('key-7d87c233-3e3b-4c60-aee0-851a715eedb8','Ym3YUf2zrmeCnu+JvZBk6aM4tMQji2NyUEc3167bSnB5Apvu0XG+pZ18e3s/+eH+jc5g2aLQ1FljLKc/8UbitAo4e37QTp+0E9rM/0Xon8p0AhCUy5GejBtoaORwojKgYxKJdmmYqX9yTVSwimivUSv74KVPJGBNpcCKpf9leXBu99Iq7qaPYlicZi+P0THkU6XRumbe81enteMaEZr+ZLv1j6SAsrJRoxz/nkrp6ghtYhVkxopPKNBRzh6stPMK6Pqb3+IMMXJD9RzlZ58LHZqIpkZm/fuGPCk+GbdaYi9zmMVd0v3eUwd9wmPbKKO08RrEwdkMQ7tpLGZrS+j8o3QYKQMvPpxVYib26IR4NEojQwuzTB5z868LY9S2ag/G1T7bfR65Ekqr7EcDPKhOP3eaVZH1Q0JtpKmPNKrLqghqh66A51SjxCqjDGJQVFddgcrrnZLcDq/8plYDLNkuS3v16l+6z/+a2XbDdk7xkPO6JgN3v2+wFsbYs5BANeHNgaSmLXL2m/P/BmiDdMw2iM0SGNfc86NzLZ2kB4oZycwl5BWPeGTzShIHh6bmF9gXeZBdoo3Anpexjosm63JzQtBK4u1gP9cUIW4eD/7ldL2e9qxHeLXHQ24MpTc8jv1ab3KjQ3ux8qYhb1twFrPV3SB9OnKWuyyaM/79Kzc1kpabGs6T8g1r7MZa1NejsSzTdlYq+7nCCI9EqyWXOmVPl1BG7Zz/ZQfheCLlDBwXKvMYTUBFnbOZRyxY5YotkPXMpD0T/gJKEKee5bEp+bsspskD7xlqrZNhZLrck11vTokZ6hcqLpz3Y8WPNsVf+KMQjTzFffdrxf8Xc7WEkLMdJAblBDB0Csr/kT+mrINb8X6ay6+8/2ZMu5YDTgCEo+pU+Wf8Lt85kLXv3ON64RaR6K4r1tshq51i25cwAh00NmFStie8XQzUINYCzAjD5o+4Dw7/m49KXNIxyBO6m6e0NDzcvJaY+rA7pTbnMTnXTD+v+k6nHWvnaWVElIqdC7Y7QIAIGD6JliyGklm8nG9CwWy7FeeiisMZJoNK2iuoj7ceYzcCaV5nXThSqZWHhDQQ/K9yEYrRsneOY1tWw3lkv1q4enzPurbbsL7fJpookOdNRBUdph6LbW6UE3h7G3AUcfsqhAs43XEBY+6LpROCVpnrC3428KUPKWfjGNib3lk=.xgHdEfKgGWo5TRyS+KgjkA==','2021-07-23 00:00:00'),('key-8ddb8003-acea-4471-ba65-8e7b73cccde9','j7oeOZDKkumd+w3ULufatBV4jhU8J1eGYV8XfURWIijEeTJgxjRab88iujfT0JGvR5jaUPx322dfy8CW8ZnYTjXS0N6LJ8JWD2yMSgZO6E507I+p/v2xz6UqHqCL0ovmvsBsfPQYMU2mwlPAqqQf1fNxJb8pg/ANj5yUDg6/J5txfFhLzjscIDwachUXrA/5znSA0WPvO/jOWJiRkIvD9QMTWxWhKfFm/5z3klptzZSqjFqB5I/th1N+dRV0u1bF8NZ16IznS5IkxTUB0V5u2AZjcg9eiIM2WjovR141T3J2fqZVLi4C3X/EweYq9cjanhCuDmTPbz4ZZej8h3u0AukFRUQH/hIVBwZ6azhXJSS0Btf81QkUUjmOr6TTm37IUcsVhrsbcYC4bmGO9VArdp665ZeKiSGPBRQWj5iFea7Js8wiQ1jglWMO473c2oXG1k2Dbt8sBqFX6PCRQMTV2ixMLrkkFwmK2VAf0+Blz6V3SfrFr2IvIzVv8r+i7U1K7cZNgh7SWDCVxuhEWfKtEe39G7T0tj2WNxapqLJYnBNCLGsEYErm4HVUWGG5QvT/a8KuglPjNOeCMZTxXwEySqFxROnTNRTHjQYjHBBgjphS0hJMCgI26sdKi0UdtTxb68ZyCHvzpR910etOmk1g2mduqPTOtiEcfwo+sJuV4QvIP58DkbPSc8v80dXRrq8PGD21ljCHk3P3LWWVseBY8j42ZY9kaM9HsyxB712+eRh7tRTjBAucxm9ncdu9VzUBtqbQxOtk/Vf3WE38UwXuf3xp8PcVAyaFKV2cSwFdIWqpzp/lSI5XgJ/exMUeLJ5qVGsSR+0f74TZYsKLy60skHXMn8yCUGITw/hNL99C7Fncv95678I1rf63m9Hd+IHIWw8e/mRnzgaFSl6IqNlv170yMqaYwBeTM+cYAhHFTaPnG1e9gN0+SU4jbaBQAaKYCKfQ5cpflvpW2xrndV6XHEBykZG7Dno+PYahmSmJYaQR48udLVeutT4oytf/WvO2ELESJPjFGnBrN4dT8yj545mgK8KSDvw2p4OVG4FiNiKetpsZqhD0ED8/VGw3F1X8Z/BcIm+BP1L4PgxX1L6t2ExaDa2NZYQF9NajkHpQ3c+kLfBgKZWUMrxeys+NIQkJKEUz04nGUMe6JyIwu6zBdud92SGqcI+GpUCZFhpR+1c=.DbLM8UF2Is0jCKh7S3W6Qw==','2022-01-17 00:00:00'),('key-d1d6bc32-27b5-43af-9aee-dd79a9922d3f','1AlwbcWi4ylB3Ide1BO6wxlUhHnAumnMhIWlDwKdkzNIKpnrayvJUXvxLQgx69I1TxkqfpyDhI8xqu90DYItke1wAo6Z8WouO81A51PRI/tx4xbl+WrN/oZxJqwy70fcJTG8Pv42zS5+oPmZWqC3p9CUtOx0fuVKVz6Ae6neg1tfWBPm5/HI+sdxkEf7vMURQUUe8/g2iJmkGGIC+1NrTud3gRIejKsfyiuZIccpyGIsJNFBkCKq61t6XZqTe6ahLqtBFFw+1yAWJXCihwFuUHPvO3mIBihsXrOxYgoGiBlSd296AGdiw4nqiXk+EREUPHM6rLopxpnx8A5U5YFOBbXwVGdsjtpX5v8PiPMvt89ZEkbszDqypn7JK+W8JN3RnmQBKdvGFlzP5YcDNvcIeJ0mTDRrvGXx0rpgkA6yPXnwgZOl+uD71zDpZmMXwhVGxnVHkJbv1WRAtlFRKKu+ft60BB6VxSDPvGR7XVQuthpqLsIwkpotwDI5bU7QrLK5fhbEzIa63YKlXfUjDmSIix+iatZtVZxw2bBic7rQx4zsnRSQT2D5z7H3SycIVekODXaJhVgx9P2iJCx69EplfFgvAqcVvw7Hgfei6xn3v8cK/bfNIcU+rsEGN6WNVg7ZR2ivJaz1XNUriwTqYGhYwvF/uREARhGdCvVVCzMtqDJ9iOl5QdNEPq69u2mqvrx9Vg8bdQKF70+IKomYnhxLYOSHn/AWHO5eTD2kvZ0PJiaYlHJkx1h5b2i5TAWN9JNV8Ct9pD70evZQNyodbuwepLlYDaV8DKbtCMtuk3ECnatdBq4dt00eX0oNAVnYGUQXW5CqLcnswt+hDMra88XdePznHZ3INXfSpahCbW4P+i7/k4g6BEE5at+YBz4IS++xa9bP0TTuIsdIHe6RBAojy3mYNL0bSTxMbBQUxWo3FOxfKa9tHkzAHuH+8U9IqY2Krao6XfJghIWUJeIpaM+g1s0rq6usyMBqhKSz4/6U8o5ZjjmLpQdZKwKvsq1/owT0PUV9GLUEsCQrJlvb7dfWbUAk9T36qhPyFYeliIEWsp7qVusEvMoqnK4NELKXGaPZblMMjpD+v26XU40U2r06q7I5Ld0URKKSGQ+E+lYfqRKvqzCDYKbOh19l+x9/yzjDPlXWxFze8v7ief4WgTj0smmlk359EvYuinrol5Umffo=.Z8EsYmQDrLeDn1z5F1I9KA==','2021-12-09 00:00:00'),('key-ef4a5ac0-0024-43a9-8338-2ea4ab625809','DUtPwKNU2s6ijHLUEcgLYoK+TBPP//6IdWjCDneGR+sSjc7W5PTTpTkG7A046VQ+oscLs4K0cr/nDRoSbEFYcvGA2OKUWlkFePnPJWB+spP1i1MlMItKdJ6SIp3tTbaqgQVcex2rJeTIGbD9ue/lIVZ0gfyaBKCuGiTLDY8z1bEHzAYT7+YgFxP0UEC5KR5IMBIotfkaVB1jPSn+4OxA4jFZ2H3LH3kRlNfXE4FWGkR6tFsc/auMr5hH6PAZ3HVw/l8Tck24znW8/7B84WuiDFy2U0aX2TiG+Fzcy7GsWkZOFi5quIN+ahHgI987BvOkPTWIoE4MeF6NQC+VhGG0iz7/rFeYc0sbWsUchsoYfB2hkKiEnbv4jaIT2BqHi1LGsYZ/cR/g0bWrOeomr700DxXnuIT1jRfaPtrD567qsZngPq7Y5EZ8qTmFQ5wuN30h0i9FZFdty5RvIwOzSqbVKDiNwThwLLVh60dtc7XVbz1gSpFwUpzyGBi5FwH44MyF9RUKB8SLEyxs36fsXvEyJWlgOZNR6EqlKpqw50TXovj6b+PJQd8YTlnWRf2K6O0WwgC7u5+yvCRVQVmgQn1rNb7DwTYqlTe+lESAA3jHzgctEIdIZcl2jK68e43WTA9+u374K6r3mCtPYXAKhqEWWctgvNU8lJyWnGmxdUQ/zhv6Fo442hKueZeWcbFWTybvROMbN3UISUlXtS3rf9lgkkXCmtyD3hqTNFbhtsxOuovuiXM+GvWfUVxHuaJNRHentRLVSjz1yNtJVoiDqimM2z3WR3X3xQNCqtyNpG+9GnI+91B6j3AAOTygF75Uq6r/0WMs8umAOxgPyRDnbEoZZ8t9F1PD+0x4otnmTS6KAO64fUGxy6tinkTN9BSquVGROtPFEPLs24SfxQfQE3f1+3LzqeT2jEau7bQHEc2N+TXv+VauVFaQBC40JjTVz8X+Kx3i0XEqTvY7cC+duYqlWxgcaImpqQf03ewyoJ/cXNRlLCelO5nufevX8lvHCTdxm6kAkchpDmmBqtGREYVGqbVF5sxPqpuyRuFi28X81mhnVUvNYvDgI8fW1KpAoXRm3UwGf2mIkrmfylh00htrAm1YQzKtNrKRB+xlJVoNzykL/XQcQZX6aIdhm0LL7GZ9VopRJWF6eLFm7leLCGamR0+R01HtwRnRMHiDK8eHdas=.14Pt79O2BPDzAWml/aVRlw==','2021-12-27 00:00:00');
/*!40000 ALTER TABLE `cms_xml_repository` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2022-01-20 20:35:11
