using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Magicodes.Admin.Core.Custom.Basis;
using System;
using System.Collections.Generic;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.LineData.Dto;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.AutoMapper;

namespace Magicodes.Admin.LineData
{
    public class LineAppService : AdminAppServiceBase
	{
		private readonly IRepository<Line, int> _lineRepository;
		private readonly IDapperRepository<Line, int> _lineDapperRepository;

		public LineAppService(IRepository<Line, int> lineRepository, IDapperRepository<Line, int> lineDapperRepository)
		{
			_lineRepository = lineRepository;
			_lineDapperRepository = lineDapperRepository;
		}

		/// <summary>
		/// 查询站点信息列表
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<PagedResultDto<LineListDto>> GetAllLineInfo(GetLineInput input)
		{
			var query = from line in _lineRepository.GetAll()
						.WhereIf(!input.Filter.IsNullOrWhiteSpace(),
						   s => s.LineName.Contains(input.Filter))
						select new LineListDto
						{
							Id = line.Id,
							LineName = line.LineName,
							IsEnable = line.IsEnable,
							Remarks = line.Remarks,
							CreatorUserId = line.CreatorUserId,
							CreationTime = line.CreationTime
						};
			var totalCount = await query.CountAsync();
			var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

			return new PagedResultDto<LineListDto>(
				totalCount,
				items);
		}
		/// <summary>
		/// 新增或修改国家
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task CreateOrUpdateLine(LineInput input)
		{
			if (!input.Id.HasValue)
			{
				await CreateLineAsync(input);
			}
			else
			{
				await UpdateLineAsync(input);
			}
		}
		/// <summary>
		/// Create 
		/// </summary>
		/// <param name="input">query parameter</param>
		/// <returns></returns>
		protected virtual async Task CreateLineAsync(LineInput input)
		{
			//判断Code是否重复
			if (_lineRepository.GetAll().Any(p => p.LineName == input.LineName))
			{
				throw new UserFriendlyException(3000, "路线已经存在！");
			}
			Line line = new Line();
			line.LineName = input.LineName;
			line.IsEnable = input.IsEnable;
			line.Remarks = input.Remarks;
			line.CreatorUserId = AbpSession.UserId;
			line.CreationTime = DateTime.Now;
			line.TenantId = AbpSession.TenantId;
			await _lineRepository.InsertAsync(line);
		}

		/// <summary>
		/// modify 
		/// </summary>
		/// <param name="input">query parameter</param>
		/// <returns></returns>
		protected virtual async Task UpdateLineAsync(LineInput input)
		{
			Debug.Assert(input.Id != null, "必须设置input.Id的值");

			var line = await _lineRepository.GetAsync(input.Id.Value);
			//判断Code是否重复
			if (_lineRepository.GetAll().Any(p => p.LineName == input.LineName && input.LineName != line.LineName))
			{
				throw new UserFriendlyException(3000, "路线代码已经存在！");
			}
			line.LineName = input.LineName;
			line.IsEnable = input.IsEnable;
			line.Remarks = input.Remarks;
			line.LastModifierUserId = AbpSession.UserId;
			line.LastModificationTime = DateTime.Now;
		}
		/// <summary>
		/// 删除国家
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public async Task BatchDeleteLine(List<int> ids)
		{
			foreach (var id in ids)
			{
				if (id != null)
				{
					var line = await _lineRepository.GetAsync(id);
					line.IsDeleted = true;
					line.DeleterUserId = AbpSession.UserId;
					line.DeletionTime = DateTime.Now;
				}
			}
		}
		/// <summary>
		/// 获取单个信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<LineInput> GetOneLine(int? id)
		{
			LineInput line;
			if (id != null)
			{
				var info = await _lineRepository.GetAsync(id.Value);
				line = info.MapTo<LineInput>();
			}
			else
			{
				line = new LineInput();

			}
			return line;
		}
	}
}
