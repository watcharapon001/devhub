using Application.Common.Interfaces;
using MediatR;

namespace Application.Projects.Queries;

public class GetProjectsHandler : IRequestHandler<GetProjectsQuery, IEnumerable<Domain.Entities.Project>>
{
    private readonly IAppDb _db;
    public GetProjectsHandler(IAppDb db) => _db = db;

    public Task<IEnumerable<Domain.Entities.Project>> Handle(GetProjectsQuery req, CancellationToken ct)
        => Task.FromResult(_db.Projects.AsEnumerable());
}
