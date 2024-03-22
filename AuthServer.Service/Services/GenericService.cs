using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data.Repositories;
using AuthServer.Service.Mapping;
using Microsoft.EntityFrameworkCore;
using Shared.Dtos;
using Shared.Entities;
using Shared.Messages;
using System.Linq.Expressions;

namespace AuthServer.Service.Services;

public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class, IEntity where TDto : class
{
    private readonly IGenericRepository<TEntity> _repository;
    private readonly IUnitOfWork _unitOfWork;
    public GenericService(GenericRepository<TEntity> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Response<TDto>> AddAsync(TDto dto)
    {
        var entity = ObjectMapper.Mapper.Map<TEntity>(dto);
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        var newDto = ObjectMapper.Mapper.Map<TDto>(entity);
        return Response<TDto>.Success(newDto, 201);
    }

    public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
    {
        var list = await _repository.GetAllAsync();
        var listDto = ObjectMapper.Mapper.Map<List<TDto>>(list);

        return Response<IEnumerable<TDto>>.Success(listDto, 200);
    }

    public async Task<Response<TDto>> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null)
        {
            return Response<TDto>.Fail(GeneralMessages.IdNotFound, 404);
        }

        var dto = ObjectMapper.Mapper.Map<TDto>(entity);

        return Response<TDto>.Success(dto, 200);
    }

    public async Task<Response<NoDataDto>> Remove(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if(entity is null)
        {
            return Response<NoDataDto>.Fail(GeneralMessages.IdNotFound, 404);
        }

        _repository.Remove(entity);
        await _unitOfWork.SaveChangesAsync();

        return Response<NoDataDto>.Success(200);
    }

    public async Task<Response<NoDataDto>> UpdateAsync(TDto dto, int id)
    {
        var isExist = await _repository.GetByIdAsync(id);
        if (isExist is null)
        {
            return Response<NoDataDto>.Fail(GeneralMessages.IdNotFound, 404);
        }

        var entity = ObjectMapper.Mapper.Map<TEntity>(dto);
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return Response<NoDataDto>.Success(204);
    }

    public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
    {
        var querable = _repository.Where(predicate);
        var list = await querable.ToListAsync();
        var listDto = ObjectMapper.Mapper.Map<IEnumerable<TDto>>(list);

        return Response<IEnumerable<TDto>>.Success(listDto, 200);
    }
}
