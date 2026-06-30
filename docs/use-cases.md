# Use Cases

This template uses a veterinary clinic domain to demonstrate Clean Architecture, vertical-slice CQRS, Minimal APIs, validation, persistence, authorization, and external-service integration.

The application supports day-to-day clinic workflows: managing clinics, veterinarians, owners, pets, appointments, vaccines, and pet vaccine administration history. It also includes account/profile endpoints and an external dog breed lookup to show how infrastructure integrations stay behind Application abstractions.

## Authentication Flows

Authentication is implemented with a BFF-oriented browser flow plus token-based service access:

- Browser/web app clients authenticate through the BFF flow and call this API with an authenticated cookie.
- Other APIs and service clients call this API with bearer access tokens.

This allows a single Web API surface to support both user-session traffic and machine-to-machine integrations.

## Use Case Summary

| Area | Use cases |
| --- | --- |
| Accounts | Login, sign out, inspect authenticated claims, retrieve profile photo. |
| Appointments | List, get, create, cancel, complete, and delete appointments. |
| Clinics | List, get, create, update, delete, and manage veterinarian assignments. |
| Dog breeds | Retrieve dog breed details from an external provider. |
| Owners | List, get, create, create with an initial pet, update, and delete owners. |
| Pets | List, get, create, update, delete, transfer ownership, and manage vaccine administrations. |
| Vaccines | List, get, create, update, and delete vaccine catalog entries. |
| Veterinarians | List, get, create, update, and delete veterinarian records. |

## Accounts

Account endpoints demonstrate authentication integration and current-user data access.

| Use case | Method and route | Application message | Notes |
| --- | --- | --- | --- |
| Login | `GET /api/accounts/login` | Endpoint-only flow | Redirects the user to the configured login flow. |
| Sign out | `GET /api/accounts/signout` | Endpoint-only flow | Signs out the current user and redirects to the signed-out callback. |
| Get user claims | `GET /api/accounts/claims` | Endpoint-only flow | Returns claims for the authenticated user. |
| Get profile photo | `GET /api/accounts/photo` | `GetPhotoQuery` | Retrieves the authenticated user's profile photo through the account photo service. |

## Appointments

Appointment use cases demonstrate scheduling plus workflow state transitions.

| Use case | Method and route | Application message | Notes |
| --- | --- | --- | --- |
| Get appointments | `GET /api/appointments` | `GetAppointmentsQuery` | Supports shared search parameters. |
| Get appointment by id | `GET /api/appointments/{id}` | `GetAppointmentByIdQuery` | Returns one appointment or a not-found result. |
| Create appointment | `POST /api/appointments` | `CreateAppointmentCommand` | Schedules a visit for a pet with a veterinarian at a clinic. |
| Cancel appointment | `POST /api/appointments/{id}/cancel` | `CancelAppointmentCommand` | Moves an appointment into the canceled state. |
| Complete appointment | `POST /api/appointments/{id}/complete` | `CompleteAppointmentCommand` | Moves an appointment into the completed state. |
| Delete appointment | `DELETE /api/appointments/{id}` | `DeleteAppointmentCommand` | Removes an appointment record. |

## Clinics

Clinic use cases demonstrate aggregate management and assignment operations.

| Use case | Method and route | Application message | Notes |
| --- | --- | --- | --- |
| Get clinics | `GET /api/clinics` | `GetClinicsQuery` | Supports shared search parameters. |
| Get clinic by id | `GET /api/clinics/{id}` | `GetClinicByIdQuery` | Returns one clinic or a not-found result. |
| Create clinic | `POST /api/clinics` | `CreateClinicCommand` | Creates a clinic with name and address. |
| Update clinic | `PUT /api/clinics/{id}` | `UpdateClinicCommand` | Updates clinic details. |
| Delete clinic | `DELETE /api/clinics/{id}` | `DeleteClinicCommand` | Removes a clinic record. |
| Add veterinarian to clinic | `POST /api/clinics/{id}/veterinarians` | `AddVeterinarianToClinicCommand` | Assigns a veterinarian to a clinic. |
| Remove veterinarian from clinic | `DELETE /api/clinics/{id}/veterinarians/{veterinarianId}` | `RemoveVeterinarianFromClinicCommand` | Removes a veterinarian assignment from a clinic. |

## Dog Breeds

Dog breed use cases demonstrate read-only external API integration.

| Use case | Method and route | Application message | Notes |
| --- | --- | --- | --- |
| Get dog breed by id | `GET /api/dog-breeds/{breedId}` | `GetDogBreedByIdQuery` | Returns dog breed details from the configured external dog API integration. |

## Owners

Owner use cases demonstrate customer record management and a composed creation flow.

| Use case | Method and route | Application message | Notes |
| --- | --- | --- | --- |
| Get owners | `GET /api/owners` | `GetOwnersQuery` | Supports shared search parameters. |
| Get owner by id | `GET /api/owners/{id}` | `GetOwnerByIdQuery` | Returns one owner or a not-found result. |
| Create owner | `POST /api/owners` | `CreateOwnerCommand` | Creates an owner contact record. |
| Create owner with initial pet | `POST /api/owners/with-initial-pet` | `CreateOwnerWithInitialPetCommand` | Creates an owner and their first pet in one workflow. |
| Update owner | `PUT /api/owners/{id}` | `UpdateOwnerCommand` | Updates owner contact details. |
| Delete owner | `DELETE /api/owners/{id}` | `DeleteOwnerCommand` | Removes an owner record. |

## Pets

Pet use cases demonstrate pet record management, ownership transfer, and nested medical history operations.

| Use case | Method and route | Application message | Notes |
| --- | --- | --- | --- |
| Get pets | `GET /api/pets` | `GetPetsQuery` | Supports shared search parameters. |
| Get pet by id | `GET /api/pets/{id}` | `GetPetQuery` | Returns one pet or a not-found result. |
| Create pet | `POST /api/pets` | `CreatePetCommand` | Creates a pet for an owner. |
| Update pet | `PUT /api/pets/{id}` | `UpdatePetCommand` | Updates pet details. |
| Delete pet | `DELETE /api/pets/{id}` | `DeletePetCommand` | Removes a pet record. |
| Transfer pet ownership | `POST /api/pets/{id}/transfer-ownership` | `TransferPetOwnershipCommand` | Moves a pet to another owner. |
| Get pet vaccine administrations | `GET /api/pets/{id}/vaccine-administrations` | `GetPetVaccineAdministrationsQuery` | Lists vaccine administrations recorded for a pet. |
| Add pet vaccine administration | `POST /api/pets/{id}/vaccine-administrations` | `AddVaccineAdministrationCommand` | Records that a vaccine was administered to a pet by a veterinarian. |
| Remove pet vaccine administration | `DELETE /api/pets/{id}/vaccine-administrations/{vaccineAdministrationId}` | `RemoveVaccineAdministrationCommand` | Removes a vaccine administration record from a pet. |

## Vaccines

Vaccine use cases demonstrate catalog management for medical records.

| Use case | Method and route | Application message | Notes |
| --- | --- | --- | --- |
| Get vaccines | `GET /api/vaccines` | `GetVaccinesQuery` | Supports shared search parameters. |
| Get vaccine by id | `GET /api/vaccines/{id}` | `GetVaccineByIdQuery` | Returns one vaccine or a not-found result. |
| Create vaccine | `POST /api/vaccines` | `CreateVaccineCommand` | Creates a vaccine catalog entry. |
| Update vaccine | `PUT /api/vaccines/{id}` | `UpdateVaccineCommand` | Updates vaccine code and name. |
| Delete vaccine | `DELETE /api/vaccines/{id}` | `DeleteVaccineCommand` | Removes a vaccine catalog entry. |

## Veterinarians

Veterinarian use cases demonstrate staff record management.

| Use case | Method and route | Application message | Notes |
| --- | --- | --- | --- |
| Get veterinarians | `GET /api/veterinarians` | `GetVeterinariansQuery` | Supports shared search parameters. |
| Get veterinarian by id | `GET /api/veterinarians/{id}` | `GetVeterinarianByIdQuery` | Returns one veterinarian or a not-found result. |
| Create veterinarian | `POST /api/veterinarians` | `CreateVeterinarianCommand` | Creates a veterinarian record with contact and license details. |
| Update veterinarian | `PUT /api/veterinarians/{id}` | `UpdateVeterinarianCommand` | Updates veterinarian contact and license details. |
| Delete veterinarian | `DELETE /api/veterinarians/{id}` | `DeleteVeterinarianCommand` | Removes a veterinarian record. |

## Cross-Cutting Behavior

Most use cases share the same architectural flow:

1. Minimal API endpoint binds route, query, or body data.
2. The endpoint sends a command or query through `IMediator`.
3. Application validators run in the mediator pipeline.
4. Command handlers stage persistence changes through Application repository abstractions.
5. The unit-of-work pipeline commits successful commands and rolls back failed commands.
6. Endpoints map `Result<T>` failures to typed HTTP results.

List queries use shared search parameters where supported. Authenticated feature groups require authorization except the dog breed lookup and account login/signout entry points.