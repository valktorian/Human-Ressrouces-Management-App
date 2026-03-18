COMPOSE_PROJECT_NAME := workforcehub
COMPOSE := docker compose -p $(COMPOSE_PROJECT_NAME) -f docker-compose.yml

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
	@echo " Stopping containers..."
	$(COMPOSE) down

kafka:
	@echo " Starting Kafka only..."
	$(COMPOSE) up -d kafka

logs:
	@$(COMPOSE) logs -f

clean:
	@echo " Removing all volumes..."
	$(COMPOSE) down -v

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
	$(COMPOSE) down -v
	$(COMPOSE) --profile full up -d
	$(MAKE) init-dbs
