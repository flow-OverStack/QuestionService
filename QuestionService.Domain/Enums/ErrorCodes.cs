namespace QuestionService.Domain.Enums;

public enum ErrorCodes
{
    //Data: 1-10
    //Reputation: 11-20
    //User: 21-30
    //Authorization: 31-40
    //Question: 41-50
    //Tags: 51-60

    LengthOutOfRange = 1,

    UserNotFound = 21,

    OperationForbidden = 31,

    QuestionNotFound = 41,
    VoteAlreadyGiven = 42,
    VoteNotFound = 43,

    TagsNotFound = 51,
    TagNotFound = 52,
}