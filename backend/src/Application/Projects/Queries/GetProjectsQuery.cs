using Domain.Entities;
using MediatR;

namespace Application.Projects.Queries;

public record GetProjectsQuery() : IRequest<IEnumerable<Project>>;
