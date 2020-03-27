using AutoMapper;
using ConsultationsProject.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultationsProject.Models.MappingProfiles
{
    public class ConsultationMappingProfile : Profile
    {
        public ConsultationMappingProfile() 
        {
            CreateMap<Consultation, ConsultationDTO>();
            CreateMap<ConsultationDTO, Consultation>();
        }
    }
}
