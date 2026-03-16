.PHONY: up up-min down kafka logs clean init-dbs reset

up:
	@echo " Starting all services (full)..."
	docker compose -f docker-compose.yml --profile full up -d

up-min:
	@echo " Starting minimal services (postgres, mongo, kafka)..."
	docker compose -f docker-compose.yml --profile minimal up -d
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

reset:
	@echo " Recreating containers and volumes from scratch..."
	docker compose -f docker-compose.yml down -v
	docker compose -f docker-compose.yml --profile minimal up -d
	$(MAKE) init-dbs
