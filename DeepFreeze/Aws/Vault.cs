using System;
using System.Collections.Generic;
using Amazon.Glacier;
using Amazon.Glacier.Model;
using Amazon.Glacier.Transfer;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using DeepFreeze.Local;

namespace DeepFreeze.Aws
{
    public class Vault
    {
        static string _topicArn;
        static string _queueUrl;
        static string _queueArn;
//        static string _vaultName = "examplevault";
        
        static AmazonSimpleNotificationService _snsClient;
        static AmazonSQS sqsClient;

        const string SQS_POLICY =
            "{" +
            "    \"Version\" : \"2008-10-17\"," +
            "    \"Statement\" : [" +
            "        {" +
            "            \"Sid\" : \"sns-rule\"," +
            "            \"Effect\" : \"Allow\"," +
            "            \"Principal\" : {" +
            "                \"AWS\" : \"*\"" +
            "            }," +
            "            \"Action\"    : \"sqs:SendMessage\"," +
            "            \"Resource\"  : \"{QuernArn}\"," +
            "            \"Condition\" : {" +
            "                \"ArnLike\" : {" +
            "                    \"aws:SourceArn\" : \"{TopicArn}\"" +
            "                }" +
            "            }" +
            "        }" +
            "    ]" +
            "}";

        public string Name { get; private set; }

        public Amazon.RegionEndpoint RegionEndpoint { get; private set; }

        public IEnumerable<Location> Locations { get; set; }

        private DescribeVaultResult _metadata;
        public DescribeVaultResult Metadata
        {
            get { return _metadata ?? (_metadata = GetMetadata()); }
        }

        public Vault(string name, Amazon.RegionEndpoint reginEndpoint)
        {
            RegionEndpoint = reginEndpoint;
            Name = name;
        }

        public bool Create()
        {
            try
            {
                var manager = new ArchiveTransferManager(RegionEndpoint);
                manager.CreateVault(Name);
                return true;
            }
            catch (AmazonGlacierException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return false;
        }

        public bool Delete()
        {
            try
            {
                var manager = new ArchiveTransferManager(RegionEndpoint);
                manager.DeleteVault(Name);
                return true;
            }
            catch (AmazonGlacierException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return false;
        }

        public string Upload(string archiveFilename, string archiveDescription = "")
        {
            var manager = new ArchiveTransferManager(RegionEndpoint);
            return manager.Upload(Name, archiveDescription, archiveFilename).ArchiveId;
        }

        private DescribeVaultResult GetMetadata()
        {
            var client = new AmazonGlacierClient(RegionEndpoint);

            var describeVaultRequest = new DescribeVaultRequest()
            {
                VaultName = Name
            };

            var describeVaultResponse = client.DescribeVault(describeVaultRequest);
            return describeVaultResponse.DescribeVaultResult;
            //            Console.WriteLine("\nVault description...");
            //            Console.WriteLine(
            //                "\nVaultName: " + describeVaultResult.VaultName +
            //                "\nVaultARN: " + describeVaultResult.VaultARN +
            //                "\nVaultCreationDate: " + describeVaultResult.CreationDate +
            //                "\nNumberOfArchives: " + describeVaultResult.NumberOfArchives +
            //                "\nSizeInBytes: " + describeVaultResult.SizeInBytes +
            //                "\nLastInventoryDate: " + describeVaultResult.LastInventoryDate
            //                );
        }

//        public string GetInventory()
//        {
//            AmazonGlacier client;
//            try
//            {
//                using (client = new AmazonGlacierClient(RegionEndpoint))
//                {
//                    // Setup SNS topic and SQS queue.
//                    SetupTopicAndQueue();
//                    GetInventory();
//                }
//                Console.WriteLine("Operations successful. To continue, press Enter");
//            }
//            catch (AmazonGlacierException e) { Console.WriteLine(e.Message); }
//            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
//            catch (Exception e) { Console.WriteLine(e.Message); }
//            finally
//            {
//                // Delete SNS topic and SQS queue.
//                _snsClient.DeleteTopic(new DeleteTopicRequest() { TopicArn = _topicArn });
//                sqsClient.DeleteQueue(new DeleteQueueRequest() { QueueUrl = _queueUrl });
//            }
//        }

        static void SetupTopicAndQueue()
        {
            _snsClient = new AmazonSimpleNotificationServiceClient(Amazon.RegionEndpoint.USEast1);
            sqsClient = new AmazonSQSClient(Amazon.RegionEndpoint.USEast1);

            var ticks = DateTime.Now.Ticks;
            _topicArn = _snsClient.CreateTopic(new CreateTopicRequest { Name = "GlacierDownload-" + ticks }).CreateTopicResult.TopicArn;
            _queueUrl = sqsClient.CreateQueue(new CreateQueueRequest() { QueueName = "GlacierDownload-" + ticks }).CreateQueueResult.QueueUrl;
            _queueArn = sqsClient.GetQueueAttributes(new GetQueueAttributesRequest() { QueueUrl = _queueUrl, AttributeName = new List<string> { "QueueArn" } }).GetQueueAttributesResult.QueueARN;

            _snsClient.Subscribe(new SubscribeRequest()
            {
                Protocol = "sqs",
                Endpoint = _queueArn,
                TopicArn = _topicArn
            });

            // Add policy to the queue so SNS can send messages to the queue.
            var policy = SQS_POLICY.Replace("{QuernArn}", _queueArn).Replace("{TopicArn}", _topicArn);
            sqsClient.SetQueueAttributes(new SetQueueAttributesRequest()
            {
                QueueUrl = _queueUrl,
                Attribute = new List<Amazon.SQS.Model.Attribute>()
        {
           new Amazon.SQS.Model.Attribute()
           {
             Name = "Policy",
             Value = policy
           }
        }
            });
        }
    }
}
