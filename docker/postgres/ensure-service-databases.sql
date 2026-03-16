SELECT 'CREATE DATABASE account_write'
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'account_write')\gexec

SELECT 'CREATE DATABASE profile_write'
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'profile_write')\gexec

SELECT 'CREATE DATABASE time_write'
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'time_write')\gexec

SELECT 'CREATE DATABASE evolution_write'
WHERE NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'evolution_write')\gexec
