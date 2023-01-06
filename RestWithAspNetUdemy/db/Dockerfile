FROM mysql:5.7.22
EXPOSE 3306
COPY ./RestWithAspNetUdemy/db/migrations/ /home/database/
COPY ./RestWithAspNetUdemy/db/dataset/ /home/database/
COPY ./RestWithAspNetUdemy/ci/init_database.sh/ /docker-entrypoint-initdb.d/init_database.sh