using FluentNHibernate.Mapping;

namespace wyspaBotWebApp.Models {
    public class CalendarEventMap : ClassMap<CalendarEvent> {
        public CalendarEventMap() {
            this.Table("CalendarEvents");
            this.Id(x => x.Id).Column("uId").GeneratedBy.Guid();
            this.Map(x => x.Name).Column("vName");
            this.Map(x => x.Place).Column("vPlace");
            this.Map(x => x.When).Column("dtWhen");
            this.Map(x => x.Added).Column("dtAdded");
            this.Map(x => x.AddedBy).Column("vAddedBy");
        }
    }
}