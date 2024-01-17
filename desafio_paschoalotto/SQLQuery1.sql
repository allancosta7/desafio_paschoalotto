-- Criar o banco de dados se não existir
CREATE DATABASE desafio_paschoalotto;

-- Usar o banco de dados recém-criado
USE desafio_paschoalotto;

-- Criar a tabela no banco de dados
CREATE TABLE resultado (
    id INT AUTO_INCREMENT PRIMARY KEY,
    wpm INT,
    teclas_corretas INT,
    teclas_incorretas INT,
    total_teclas INT AS (teclas_corretas + teclas_incorretas) STORED,
    accuracy DECIMAL(5,2),
    palavras_corretas INT,
    palavras_incorretas INT
);

CREATE TABLE resultado2 (
    id INT AUTO_INCREMENT PRIMARY KEY,
    result text

);