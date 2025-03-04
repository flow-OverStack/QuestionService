namespace QuestionService.Domain.Enums;

public enum ErrorCodes
{
    //Data: 1-10
    //Question: 11-20
    //User: 21-30
    //Authorization: 31-40

    LengthOutOfRange = 1,

    QuestionNotFound = 11,

    UserNotFound = 21,

    OperationForbidden = 31
}