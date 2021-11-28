namespace TeisterMask
{
    using AutoMapper;
    using System.Linq;
    using TeisterMask.Data.Models;
    using TeisterMask.DataProcessor.ExportDto;

    public class TeisterMaskProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE OR RENAME THIS CLASS
        public TeisterMaskProfile()
        {
            CreateMap<Task, TaskOutputDTOXML>()
                .ForMember(d => d.Label, y => y.MapFrom(s => s.LabelType.ToString()));

            CreateMap<Project, ProjectOutputDTO>()
                .ForMember(d => d.HasEndDate, y => y.MapFrom(s => s.DueDate.HasValue ? "Yes" : "No"))
                .ForMember(d => d.TasksCount, y => y.MapFrom(s => s.Tasks.Count))
                .ForMember(d=>d.Tasks, y=>y.MapFrom(s=>s.Tasks.OrderBy(t=>t.Name)));
        }
    }
}
