using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using wyspaBotWebApp.Core;
using wyspaBotWebApp.Models;
using wyspaBotWebApp.Services.Calendar;

namespace WyspaBotWebAppTests.Services.Calendar {
    [TestFixture]
    public class CalendarServiceFixture {
        private ICalendarService testee;

        private IRepository<CalendarEvent> fakeRepository;

        private CalendarEvent calendarEvent1;

        private CalendarEvent calendarEvent2;

        private CalendarEvent calendarEvent3;

        private CalendarEvent calendarEvent4;

        [SetUp]
        public void SetUp() {
            this.calendarEvent1 = new CalendarEvent {
                Id = Guid.NewGuid(),
                Place = "some place",
                Added = DateTime.Now,
                When = DateTime.Now.AddDays(5),
                AddedBy = "me",
                Name = "party"
            };

            this.calendarEvent2 = new CalendarEvent {
                Id = Guid.NewGuid(),
                Place = "some place for 2nd event",
                Added = DateTime.Now,
                When = DateTime.Now.AddDays(2),
                AddedBy = "w/e",
                Name = "tea time"
            };

            this.calendarEvent3 = new CalendarEvent {
                Id = Guid.NewGuid(),
                Place = "another place",
                Added = DateTime.Now,
                When = DateTime.Now.AddDays(-1),
                AddedBy = "you",
                Name = "party party"
            };

           this.calendarEvent4 = new CalendarEvent {
                Id = Guid.NewGuid(),
                Place = "yet another place",
                Added = DateTime.Now,
                When = DateTime.Now.AddDays(2).AddMinutes(1),
                AddedBy = "lol",
                Name = "party hard"
            };

            this.fakeRepository = MockRepository.GenerateMock<IRepository<CalendarEvent>>();
            this.fakeRepository.Stub(x => x.GetAll()).Return(new List<CalendarEvent> {calendarEvent1, calendarEvent2, calendarEvent3, calendarEvent4}.AsQueryable());
            this.testee = new CalendarService(this.fakeRepository);
        }


        [Test]
        public void GetNextEntry_Works() {
            var dto = this.testee.GetNextEntry();

            Assert.That(dto.Id, Is.EqualTo(this.calendarEvent2.Id));
            Assert.That(dto.When, Is.EqualTo(this.calendarEvent2.When));
            Assert.That(dto.Name, Is.EqualTo(this.calendarEvent2.Name));
            Assert.That(dto.Place, Is.EqualTo(this.calendarEvent2.Place));
            Assert.That(dto.Added, Is.EqualTo(this.calendarEvent2.Added));
            Assert.That(dto.AddedBy, Is.EqualTo(this.calendarEvent2.AddedBy));
        }

        [Test]
        public void GetAllEntries_Works() {
            var allEntries = this.testee.GetAllEntries().ToList();

            Assert.That(allEntries, Is.Not.Null);
            Assert.That(allEntries, Is.Not.Empty);
            Assert.That(allEntries.Count, Is.EqualTo(3));
            Assert.That(allEntries.Any(x => x.Id == this.calendarEvent1.Id));
            Assert.That(allEntries.Any(x => x.Id == this.calendarEvent2.Id));
            Assert.That(allEntries.All(x => x.Id != this.calendarEvent3.Id));
            Assert.That(allEntries.Any(x => x.Id == this.calendarEvent4.Id));
        }
    }
}