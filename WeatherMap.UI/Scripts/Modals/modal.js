////Definitions:
//SUC or OK = Success type
//ERR or FAIL = Error type

////how to call examples:

////add your own msg:
//alertModal('This is a success message!', 'SUC');
//alertModal('This is an error message!', 'ERR');

////or leave empty and get a generic "Operation Succeeded / Failed" msg
//alertModal('', 'SUC');
//alertModal('', 'ERR');

////or create a string and pass the variable in as the message:
//var happyMsg = 'Operation wildly successful. '
//alertModal(happyMsg, 'SUC');

////or just add the response variables to the call:
//alertModal(response.Message, response.Status);

// modal.js
function alertModal(message, status) {
  // Set default messages based on status
  var defaultMessage = status === 'FAIL' ? 'Operation failed.' : 'Operation succeeded.';
  // Use default message if 'message' parameter is empty
  message = message || defaultMessage;

  // Determine modal color based on status
  var modalColor = status === 'FAIL' ? '#ff4d4d' : '#4CAF50'; // Red for FAIL, green for OK/SUC

  // Create modal HTML
  var modalHTML = `
  <div id="alertModal" class="modal" tabindex="-1" role="dialog" style="display:block; background-color: rgba(0,0,0,0.5);">
      <div class="modal-dialog" role="document">
          <div class="modal-content" style="border-left: 5px solid ${modalColor};">
              <div class="modal-header">
                  <h5 class="modal-title">${status === 'FAIL' ? 'Error' : 'Success'}</h5>
                  <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                      <span aria-hidden="true">×</span>
                  </button>
              </div>
              <div class="modal-body">
                  <p>${message}</p>
              </div>
              <div class="modal-footer">
                  <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
              </div>
          </div>
      </div>
  </div>
  `;

  // Append modal HTML to the body
  $('body').append(modalHTML);

  // Close modal on clicking close button
  $('.close, [data-dismiss="modal"]').on('click', function () {
    $('#alertModal').remove();
  });

  // Close modal on pressing ESC key
  $(document).on('keydown', function (e) {
    if (e.key === 'Escape') {
      $('#alertModal').remove();
    }
  });

  // Close modal when clicking outside of the modal content
  $(document).on('click', function (event) {
    var $target = $(event.target);
    if (!$target.closest('.modal-content').length && $('.modal').is(':visible')) {
      $('#alertModal').remove();
    }
  });
}