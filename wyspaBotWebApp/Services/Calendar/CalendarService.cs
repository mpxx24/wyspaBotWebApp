﻿using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Dtos;
using wyspaBotWebApp.Models;

namespace wyspaBotWebApp.Services.Calendar {
    public class CalendarService : ICalendarService {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IRepository<CalendarEvent> repository;

        public CalendarService(IRepository<CalendarEvent> repository) {
            this.repository = repository;
        }

        public void AddEntry(CalendarEventDto dto) {
            try {
                var calendarEvent = new CalendarEvent {
                    Name = dto.Name,
                    Place = dto.Place,
                    When = dto.When,
                    Added = DateTime.Now,
                    AddedBy = dto.AddedBy
                };
                this.repository.Save(calendarEvent);
                this.logger.Debug($"Added new calendar entry: Name {dto.Name}, date {dto.When.ToShortDateString()}");
            }
            catch (Exception e) {
                this.logger.Debug(e, "Failed to add new calendar entry!");
                throw;
            }
        }

        public IEnumerable<CalendarEventDto> GetAllEntries() {
            try {
                var allEntries = this.repository.GetAll().Where(x => x.When > DateTime.Now).ToList();
                this.logger.Debug($"Found {allEntries.Count} calendar entries.");

                return allEntries.Select(x => new CalendarEventDto {
                    Id = x.Id,
                    Name = x.Name,
                    When = x.When,
                    Place = x.Place,
                    Added = x.Added,
                    AddedBy = x.AddedBy
                });
            }
            catch (Exception e) {
                this.logger.Debug(e, "Failed to get all calendar entries!");
                throw;
            }
        }

        public CalendarEventDto GetNextEntry() {
            try {
                var closestInTimeEntry = this.repository.GetAll().Where(x => x.When > DateTime.Now && Math.Abs(DateTime.Now.Ticks - x.When.Date.Ticks) > 0)
                                             .OrderBy(x => x.When)
                                             .FirstOrDefault();

                //TODO: null
                if (closestInTimeEntry == null) {
                    return null;
                }

                return new CalendarEventDto {
                    Id = closestInTimeEntry.Id,
                    When = closestInTimeEntry.When,
                    Place = closestInTimeEntry.Place,
                    Name = closestInTimeEntry.Name,
                    Added = closestInTimeEntry.Added,
                    AddedBy = closestInTimeEntry.AddedBy
                };
            }
            catch (Exception e) {
                this.logger.Debug(e, "Failed to get the next entry!");
                throw;
            }
        }
    }
}