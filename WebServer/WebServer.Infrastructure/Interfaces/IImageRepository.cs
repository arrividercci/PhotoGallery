﻿using WebServer.Domain.Entities;

namespace WebServer.Infrastructure.Interfaces
{
    public interface IImageRepository : IRepository<Image, int>
    {
    }
}
