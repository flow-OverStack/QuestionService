using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services;

public class TagService(IBaseRepository<Tag> tagRepository, IMapper mapper, IOptions<BusinessRules> businessRules)
    : ITagService
{
    private readonly BusinessRules _businessRules = businessRules.Value;

    public async Task<BaseResult<TagDto>> CreateTagAsync(CreateTagDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsValidData(dto))
            return BaseResult<TagDto>.Failure(ErrorMessage.LengthOutOfRange, (int)ErrorCodes.LengthOutOfRange);

        var tag = await tagRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name, cancellationToken);
        if (tag != null)
            return BaseResult<TagDto>.Failure(ErrorMessage.TagAlreadyExists, (int)ErrorCodes.TagAlreadyExists);

        tag = mapper.Map<Tag>(dto);

        await tagRepository.CreateAsync(tag, cancellationToken);
        await tagRepository.SaveChangesAsync(cancellationToken);

        return BaseResult<TagDto>.Success(mapper.Map<TagDto>(tag));
    }

    public async Task<BaseResult<TagDto>> UpdateTagAsync(TagDto dto, CancellationToken cancellationToken = default)
    {
        if (!IsValidData(dto))
            return BaseResult<TagDto>.Failure(ErrorMessage.LengthOutOfRange, (int)ErrorCodes.LengthOutOfRange);

        var tag = await tagRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
        if (tag == null)
            return BaseResult<TagDto>.Failure(ErrorMessage.TagNotFound, (int)ErrorCodes.TagNotFound);

        mapper.Map(dto, tag);

        tagRepository.Update(tag);

        await tagRepository.SaveChangesAsync(cancellationToken);

        return BaseResult<TagDto>.Success(dto);
    }

    public async Task<BaseResult<TagDto>> DeleteTagAsync(long id, CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (tag == null)
            return BaseResult<TagDto>.Failure(ErrorMessage.TagNotFound, (int)ErrorCodes.TagNotFound);

        tagRepository.Remove(tag);
        await tagRepository.SaveChangesAsync(cancellationToken);

        return BaseResult<TagDto>.Success(mapper.Map<TagDto>(tag));
    }

    private bool IsValidData(CreateTagDto dto)
    {
        return dto.Name.HasMaxLength(_businessRules.TagMaxLength) &&
               dto.Description.HasMaxLength(_businessRules.TagDescriptionMaxLength);
    }

    private bool IsValidData(TagDto dto)
    {
        return dto.Name.HasMaxLength(_businessRules.TagMaxLength) &&
               dto.Description.HasMaxLength(_businessRules.TagDescriptionMaxLength);
    }
}