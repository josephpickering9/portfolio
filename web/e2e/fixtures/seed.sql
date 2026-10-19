-- Fixed data for the E2E suite in CI: enough companies, technologies and projects for every
-- public page to render real content (featured work, filters, timeline, case study, gallery).

INSERT INTO "Company" ("Id", "Name", "Slug", "Description", "StartDate", "EndDate", "Website") VALUES
  ('10000000-0000-0000-0000-000000000001', 'Personal', 'personal', 'Personal projects.', NULL, NULL, NULL),
  ('10000000-0000-0000-0000-000000000002', 'Acme Studio', 'acme-studio', 'A digital agency.', '2019-01-01', '2023-06-01', 'https://acme.example');

INSERT INTO "Tag" ("Id", "Title", "Slug", "Type", "Icon", "CustomColour") VALUES
  ('20000000-0000-0000-0000-000000000001', 'TypeScript', 'typescript', 'Frontend', 'simple-icons:typescript', NULL),
  ('20000000-0000-0000-0000-000000000002', 'JavaScript', 'javascript', 'Frontend', 'simple-icons:javascript', NULL),
  ('20000000-0000-0000-0000-000000000003', 'Vue.js', 'vuejs', 'Frontend', 'simple-icons:vuedotjs', NULL),
  ('20000000-0000-0000-0000-000000000004', '.NET', 'net', 'Backend', 'simple-icons:dotnet', NULL),
  ('20000000-0000-0000-0000-000000000005', 'PostgreSQL', 'postgresql', 'Data', 'simple-icons:postgresql', NULL),
  ('20000000-0000-0000-0000-000000000006', 'Swift', 'swift', 'Mobile', 'simple-icons:swift', NULL);

INSERT INTO "Project" ("Id", "Title", "Slug", "ShortDescription", "Description", "ProblemStatement", "KeyDecision", "Outcome", "CompanyId", "StartDate", "EndDate", "Website", "ShowMockup") VALUES
  ('30000000-0000-0000-0000-000000000001', 'Pulse', 'pulse', 'A health dashboard for distributed teams.', '<p>Pulse collects check-ins and turns them into a weekly report.</p>', 'Teams had no shared view of workload.', 'Server-rendered dashboards over a typed API.', 'Adopted by every team in the pilot.', '10000000-0000-0000-0000-000000000001', '2025-03-01', NULL, 'https://pulse.example', true),
  ('30000000-0000-0000-0000-000000000002', 'Portfolio', 'portfolio', 'This site: Nuxt 4, a .NET 10 API and AI search.', '<p>The portfolio you are looking at.</p>', NULL, NULL, NULL, '10000000-0000-0000-0000-000000000001', '2024-01-01', NULL, NULL, false),
  ('30000000-0000-0000-0000-000000000003', 'Booking Engine', 'booking-engine', 'Online booking for a visitor attraction.', '<p>A booking engine with timed tickets.</p>', NULL, NULL, NULL, '10000000-0000-0000-0000-000000000002', '2021-05-01', '2022-02-01', NULL, false),
  ('30000000-0000-0000-0000-000000000004', 'Field App', 'field-app', 'An offline-first iOS app for site surveys.', '<p>Survey forms that sync when back online.</p>', NULL, NULL, NULL, '10000000-0000-0000-0000-000000000002', '2020-01-01', '2020-12-01', NULL, false);

INSERT INTO "ProjectTag" ("ProjectsId", "TagsId") VALUES
  ('30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001'),
  ('30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000003'),
  ('30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000004'),
  ('30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000001'),
  ('30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000004'),
  ('30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000005'),
  ('30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000002'),
  ('30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000005'),
  ('30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000006');

INSERT INTO "ProjectStat" ("Id", "ProjectId", "Label", "Value", "Order") VALUES
  ('40000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', 'Teams', '12', 0),
  ('40000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000001', 'Weekly check-ins', '300+', 1);

INSERT INTO "ProjectImage" ("Id", "ProjectId", "Image", "Type", "Order", "IsOptimised") VALUES
  ('50000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', 'e2e-card.png', 'Card', NULL, true),
  ('50000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000001', 'e2e-desktop-1.png', 'Desktop', 1, true),
  ('50000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000001', 'e2e-desktop-2.png', 'Desktop', 2, true);
