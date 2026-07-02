using ProjectNetIa.Application.DTOs;
using ProjectNetIa.Application.Interfaces;

namespace ProjectNetIa.Application.Services;

public sealed class SampleService : ISampleService
{
    public SampleDto GetSample()
    {
        return new SampleDto
        {
            Id = Guid.NewGuid(),
            Name = "ProjectNetIa"
        };
    }
}
