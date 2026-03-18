COMPOSE_PROJECT_NAME := workforcehub
COMPOSE := docker compose -p $(COMPOSE_PROJECT_NAME) -f docker-compose.yml
CURRENT_DIR_PROJECT := $(shell basename "$(CURDIR)" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9]/-/g')
CONTAINERS := workforcehub-gateway account-service-command account-service-query profile-service-command profile-service-query time-service-command time-service-query evolution-service-command evolution-service-query postgres_write mongo_read kafka adminer mongo_express kafka-ui

.PHONY: up up-min up-fresh down kafka logs clean init-dbs reset build-gateway rebuild-gateway

up:
	@echo " Starting all services (full)..."
	$(COMPOSE) --profile full up -d
	@echo " Ensuring per-service write databases exist..."
	$(MAKE) init-dbs

up-min:
	@echo " Starting minimal stack (postgres, mongo, kafka, admin tools, gateway)..."
	$(COMPOSE) --profile minimal up -d
	@echo " Ensuring per-service write databases exist..."
	$(MAKE) init-dbs

up-fresh: rebuild-gateway
	@echo " Starting full stack with a freshly rebuilt gateway..."
	$(COMPOSE) --profile full up -d
	@echo " Ensuring per-service write databases exist..."
	$(MAKE) init-dbs

down:
	@echo " Stopping containers and removing the stack..."
	-$(COMPOSE) down --remove-orphans
	-docker compose -p $(CURRENT_DIR_PROJECT) -f docker-compose.yml down --remove-orphans
	-docker compose -f docker-compose.yml down --remove-orphans
	-docker rm -f $(CONTAINERS) 2>/dev/null || true
	-docker network rm $(COMPOSE_PROJECT_NAME)_default $(CURRENT_DIR_PROJECT)_default hrmapp_default 2>/dev/null || true

kafka:
	@echo " Starting Kafka only..."
	$(COMPOSE) up -d kafka

logs:
	@$(COMPOSE) logs -f

clean:
	@echo " Removing all volumes..."
	-$(COMPOSE) down -v --remove-orphans
	-docker compose -p $(CURRENT_DIR_PROJECT) -f docker-compose.yml down -v --remove-orphans
	-docker compose -f docker-compose.yml down -v --remove-orphans
	-docker rm -f $(CONTAINERS) 2>/dev/null || true
	-docker volume rm $(COMPOSE_PROJECT_NAME)_pg_write_data $(COMPOSE_PROJECT_NAME)_mongo_data $(COMPOSE_PROJECT_NAME)_kafka_data $(CURRENT_DIR_PROJECT)_pg_write_data $(CURRENT_DIR_PROJECT)_mongo_data $(CURRENT_DIR_PROJECT)_kafka_data hrmapp_pg_write_data hrmapp_mongo_data hrmapp_kafka_data 2>/dev/null || true
	-docker network rm $(COMPOSE_PROJECT_NAME)_default $(CURRENT_DIR_PROJECT)_default hrmapp_default 2>/dev/null || true

init-dbs:
	@echo " Ensuring account/profile/time/evolution databases exist..."
	docker exec -i postgres_write psql -U admin -d postgres < docker/postgres/ensure-service-databases.sql

build-gateway:
	@echo " Building workforcehub-gateway image..."
	$(COMPOSE) --profile full build workforcehub-gateway

rebuild-gateway:
	@echo " Rebuilding and recreating workforcehub-gateway..."
	$(COMPOSE) --profile full build --no-cache workforcehub-gateway
	$(COMPOSE) --profile full up -d --force-recreate workforcehub-gateway

reset:
	@echo " Recreating containers and volumes from scratch..."
	$(MAKE) clean
	$(COMPOSE) --profile full up -d
	$(MAKE) init-dbs
