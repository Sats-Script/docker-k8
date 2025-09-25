CREATE TABLE "Products" (
  "Id" serial PRIMARY KEY,
  "Name" text NOT NULL,
  "Price" numeric(10,2) NOT NULL,
  "InStock" boolean NOT NULL
);

INSERT INTO "Products" ("Name","Price","InStock") VALUES
  ('Notebook', 199.00, true),
  ('Pen', 25.50, true);
