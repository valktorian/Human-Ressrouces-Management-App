.PHONY: up up-min up-fresh down kafka logs clean init-dbs reset build-gateway rebuild-gateway

up:
	@echo " Starting all services (full)..."
	docker compose -f docker-compose.yml --profile full up -d
	@echo " Ensuring per-service write databases exist..."
	$(MAKE) init-dbs

up-min:
	@echo " Starting infrastructure only (postgres, mongo, kafka, admin tools)..."
	docker compose -f docker-compose.yml --profile minimal up -d
	@echo " Ensuring per-service write databases exist..."
	$(MAKE) init-dbs

up-fresh: rebuild-gateway
	@echo " Starting full stack with a freshly rebuilt gateway..."
	docker compose -f docker-compose.yml --profile full up -d
	@echo " Ensuring per-service write databases exist..."
	$(MAKE) init-dbs

down:
	@echo " Stopping containers..."
	docker compose -f docker-compose.yml down

kafka:
	@echo " Starting Kafka only..."
	docker compose -f docker-compose.yml up -d kafka

logs:
	@docker compose logs -f

clean:
	@echo " Removing all volumes..."
	docker compose down -v

init-dbs:
	@echo " Ensuring account/profile/time/evolution databases exist..."
	docker exec -i postgres_write psql -U admin -d postgres < docker/postgres/ensure-service-databases.sql

build-gateway:
	@echo " Building workforcehub-gateway image..."
	docker compose -f docker-compose.yml --profile full build workforcehub-gateway

rebuild-gateway:
	@echo " Rebuilding and recreating workforcehub-gateway..."
	docker compose -f docker-compose.yml --profile full build --no-cache workforcehub-gateway
	docker compose -f docker-compose.yml --profile full up -d --force-recreate workforcehub-gateway

reset:
	@echo " Recreating containers and volumes from scratch..."
	docker compose -f docker-compose.yml down -v
	docker compose -f docker-compose.yml --profile full up -d
	$(MAKE) init-dbs
